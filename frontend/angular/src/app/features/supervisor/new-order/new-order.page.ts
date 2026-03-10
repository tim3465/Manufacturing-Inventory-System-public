import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomersApi } from '../../../core/api/customers.api';
import { MachinesApi } from '../../../core/api/machines.api';
import { OrderPlanningApi } from '../../../core/api/order-planning.api';
import { PartsApi } from '../../../core/api/parts.api';
import { StockLotsApi } from '../../../core/api/stock-lots.api';
import { CustomerDto } from '../../../core/dtos/customers/customer.dto';
import { MachineDto } from '../../../core/dtos/machines/machine.dto';
import { CreateJobInOrderRequestDto, CreateOrderWithJobsRequestDto } from '../../../core/dtos/order-planning/order-planning.dto';
import { CreatePartRequestDto } from '../../../core/dtos/parts/create-part-request.dto';
import { PartDto } from '../../../core/dtos/parts/part.dto';
import { StockLotDto } from '../../../core/dtos/stock-lots/stock-lot.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';

interface RawJobValue {
  stockLotId: string | number;
  machineId: string | number;
  partAmountPlanned: string | number;
  barAmountPlanned: string | number;
  barCycleTime: string;
  barsInJob: string | number;
  estimatedPartsPerBar: string | number | null;
  dueDate: string;
}

@Component({
  selector: 'app-new-order-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './new-order.page.html',
  styleUrl: './new-order.page.css'
})
export class NewOrderPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly customersApi = inject(CustomersApi);
  private readonly partsApi = inject(PartsApi);
  private readonly stockLotsApi = inject(StockLotsApi);
  private readonly machinesApi = inject(MachinesApi);
  private readonly orderPlanningApi = inject(OrderPlanningApi);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly customers = signal<CustomerDto[]>([]);
  protected readonly parts = signal<PartDto[]>([]);
  protected readonly stockLots = signal<StockLotDto[]>([]);
  protected readonly machines = signal<MachineDto[]>([]);

  protected readonly submitting = signal(false);
  protected readonly successOrderId = signal<number | null>(null);

  // Two-phase state
  protected readonly phase1Confirmed = signal(false);
  protected readonly showNewPartFields = signal(false);

  // Phase 1 form
  protected readonly phase1Form = this.fb.nonNullable.group({
    customerId: [0, [Validators.required, Validators.min(1)]],
    partMode: ['existing' as 'existing' | 'new'],
    partId: [0],
    newPartName: [''],
    newPartNumber: [''],
    newPartCycleTime: ['00:00:30'],
    newPartCheckPerPart: [1],
    partAmountRequested: [1, [Validators.required, Validators.min(1)]],
    partsPerBar: [1, [Validators.required, Validators.min(1)]]
  });

  // Phase 2 job form array
  protected readonly jobsForm = this.fb.nonNullable.group({
    jobs: this.fb.array([this.createJobGroup()])
  });

  get jobsArray(): FormArray {
    return this.jobsForm.get('jobs') as FormArray;
  }

  getJobGroup(index: number): FormGroup {
    return this.jobsArray.at(index) as FormGroup;
  }

  getControl(group: AbstractControl, name: string) {
    return (group as FormGroup).get(name);
  }

  // Derived: resolved partId (from existing or newly created)
  protected resolvedPartId = signal<number>(0);

  protected readonly isNewPartMode = computed(() => this.showNewPartFields());

  ngOnInit(): void {
    this.customersApi.listActive().subscribe({
      next: (data) => this.customers.set(data),
      error: () => this.toast.error('Failed to load customers')
    });

    this.partsApi.listActive().subscribe({
      next: (data) => this.parts.set(data),
      error: () => this.toast.error('Failed to load parts')
    });

    this.stockLotsApi.listActive().subscribe({
      next: (data) => this.stockLots.set(data),
      error: () => this.toast.error('Failed to load stock lots')
    });

    this.machinesApi.listAll().subscribe({
      next: (data) => this.machines.set(data.filter((m) => !m.inactivatedDateTime)),
      error: () => this.toast.error('Failed to load machines')
    });
  }

  protected togglePartMode(mode: 'existing' | 'new'): void {
    this.phase1Form.controls.partMode.setValue(mode);
    this.showNewPartFields.set(mode === 'new');

    if (mode === 'existing') {
      this.phase1Form.controls.newPartName.clearValidators();
      this.phase1Form.controls.newPartNumber.clearValidators();
      this.phase1Form.controls.newPartCycleTime.clearValidators();
      this.phase1Form.controls.partId.setValidators([Validators.required, Validators.min(1)]);
    } else {
      this.phase1Form.controls.partId.clearValidators();
      this.phase1Form.controls.newPartName.setValidators([Validators.required, Validators.maxLength(100)]);
      this.phase1Form.controls.newPartNumber.setValidators([Validators.required, Validators.maxLength(50)]);
      this.phase1Form.controls.newPartCycleTime.setValidators([
        Validators.required,
        Validators.pattern(/^\d{2}:\d{2}:\d{2}$/)
      ]);
    }

    this.phase1Form.controls.partId.updateValueAndValidity();
    this.phase1Form.controls.newPartName.updateValueAndValidity();
    this.phase1Form.controls.newPartNumber.updateValueAndValidity();
    this.phase1Form.controls.newPartCycleTime.updateValueAndValidity();
  }

  protected onConfirmPhase1(): void {
    // Validate only the relevant phase1 controls
    const controls = this.phase1Form.controls;
    const relevant: AbstractControl[] = [
      controls.customerId,
      controls.partAmountRequested,
      controls.partsPerBar
    ];

    if (this.isNewPartMode()) {
      relevant.push(controls.newPartName, controls.newPartNumber, controls.newPartCycleTime);
    } else {
      relevant.push(controls.partId);
    }

    relevant.forEach((c) => c.markAsTouched());
    const anyInvalid = relevant.some((c) => c.invalid);
    if (anyInvalid) return;

    if (!this.isNewPartMode()) {
      this.resolvedPartId.set(Number(controls.partId.value));
      this.phase1Confirmed.set(true);
    } else {
      // Create the part first, then confirm
      const dto: CreatePartRequestDto = {
        partName: controls.newPartName.value.trim(),
        partNumber: controls.newPartNumber.value.trim(),
        approxPartCycleTime: controls.newPartCycleTime.value.trim(),
        checkPerPart: Number(controls.newPartCheckPerPart.value)
      };

      this.partsApi.create(dto).subscribe({
        next: (created) => {
          this.resolvedPartId.set(created.id);
          // Refresh parts list and set partId
          this.partsApi.listActive().subscribe({ next: (data) => this.parts.set(data) });
          this.phase1Confirmed.set(true);
          this.toast.success(`Part "${dto.partName}" created`);
        },
        error: (err: unknown) => {
          this.toast.errorMessage(err, undefined, 'Failed to create part');
        }
      });
    }
  }

  protected onEditPhase1(): void {
    if (this.jobsArray.length > 0) {
      const confirmed = window.confirm(
        'You have jobs defined. Editing the order details will clear them. Are you sure?'
      );
      if (!confirmed) return;

      // Clear jobs back to one empty group
      while (this.jobsArray.length > 0) {
        this.jobsArray.removeAt(0);
      }
      this.jobsArray.push(this.createJobGroup());
    }
    this.phase1Confirmed.set(false);
  }

  protected addJob(): void {
    this.jobsArray.push(this.createJobGroup());
  }

  protected removeJob(index: number): void {
    if (this.jobsArray.length > 1) {
      this.jobsArray.removeAt(index);
    }
  }

  protected onSubmit(): void {
    if (this.submitting()) return;

    if (this.jobsForm.invalid) {
      this.jobsForm.markAllAsTouched();
      return;
    }

    const p1 = this.phase1Form.getRawValue();
    const raw = this.jobsForm.getRawValue();

    const jobs: CreateJobInOrderRequestDto[] = (raw.jobs as unknown as RawJobValue[]).map((j) => ({
      stockLotId: Number(j.stockLotId),
      machineId: Number(j.machineId),
      partAmountPlanned: Number(j.partAmountPlanned),
      barAmountPlanned: Number(j.barAmountPlanned),
      barCycleTime: String(j.barCycleTime).trim(),
      barsInJob: Number(j.barsInJob),
      estimatedPartsPerBar:
        j.estimatedPartsPerBar !== null && j.estimatedPartsPerBar !== ''
          ? Number(j.estimatedPartsPerBar)
          : null,
      dueDate: String(j.dueDate)
    }));

    const dto: CreateOrderWithJobsRequestDto = {
      customerId: Number(p1.customerId),
      partId: this.resolvedPartId(),
      partAmountRequested: Number(p1.partAmountRequested),
      partsPerBar: Number(p1.partsPerBar),
      jobs
    };

    this.submitting.set(true);
    this.orderPlanningApi.createOrderWithJobs(dto).subscribe({
      next: (response) => {
        this.toast.success(`Order #${response.orderId} created with ${response.jobIds.length} job(s)`);
        this.successOrderId.set(response.orderId);
        this.submitting.set(false);
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to create order');
        this.submitting.set(false);
      }
    });
  }

  protected goToProduction(): void {
    this.router.navigate(['/supervisor/production']);
  }

  protected resetForm(): void {
    this.successOrderId.set(null);
    this.phase1Confirmed.set(false);
    this.showNewPartFields.set(false);
    this.resolvedPartId.set(0);

    this.phase1Form.reset({
      customerId: 0,
      partMode: 'existing',
      partId: 0,
      newPartName: '',
      newPartNumber: '',
      newPartCycleTime: '00:00:30',
      newPartCheckPerPart: 1,
      partAmountRequested: 1,
      partsPerBar: 1
    });

    while (this.jobsArray.length > 0) {
      this.jobsArray.removeAt(0);
    }
    this.jobsArray.push(this.createJobGroup());
  }

  private createJobGroup(): FormGroup {
    return this.fb.nonNullable.group({
      stockLotId: ['', [Validators.required, Validators.min(1)]],
      machineId: ['', [Validators.required, Validators.min(1)]],
      partAmountPlanned: ['', [Validators.required, Validators.min(0)]],
      barAmountPlanned: ['', [Validators.required, Validators.min(0)]],
      barCycleTime: ['', [Validators.required, Validators.pattern(/^\d{2}:\d{2}:\d{2}$/)]],
      barsInJob: ['', [Validators.required, Validators.min(0)]],
      estimatedPartsPerBar: [''],
      dueDate: ['', [Validators.required]]
    });
  }
}
