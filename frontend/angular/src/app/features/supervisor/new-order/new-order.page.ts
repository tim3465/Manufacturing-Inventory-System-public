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
  stockLotId: string | number | null;
  machineId: string | number;
  partAmountPlanned: string | number;
  barAmountPlanned: string | number;
  barCycleTime: string;
  barLoaderTime: string;
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

  // Resolved part (set on confirm so Phase 2 can use part data)
  protected readonly resolvedPart = signal<PartDto | null>(null);

  // Derived: resolved partId (from existing or newly created)
  protected resolvedPartId = signal<number>(0);

  protected readonly isNewPartMode = computed(() => this.showNewPartFields());

  // Phase 1 form — includes numberOfJobs as last field
  protected readonly phase1Form = this.fb.nonNullable.group({
    customerId: [0, [Validators.required, Validators.min(1)]],
    partMode: ['existing' as 'existing' | 'new'],
    partId: [0],
    newPartName: [''],
    newPartNumber: [''],
    newPartCycleTime: ['00:00:30'],
    newPartCheckPerPart: [1],
    partAmountRequested: [1, [Validators.required, Validators.min(1)]],
    partsPerBar: [1, [Validators.required, Validators.min(1)]],
    numberOfJobs: [1, [Validators.required, Validators.min(1)]]
  });

  // Computed: estimated bars needed from Phase 1 inputs
  protected readonly estimatedBarsNeeded = computed(() => {
    const req = Number(this.phase1Form.controls.partAmountRequested.value) || 0;
    const ppb = Number(this.phase1Form.controls.partsPerBar.value) || 1;
    return Math.ceil(req / ppb);
  });

  // Phase 2 job form array
  protected readonly jobsForm = this.fb.nonNullable.group({
    jobs: this.fb.array([] as FormGroup[])
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
    const controls = this.phase1Form.controls;
    const relevant: AbstractControl[] = [
      controls.customerId,
      controls.partAmountRequested,
      controls.partsPerBar,
      controls.numberOfJobs
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
      const partId = Number(controls.partId.value);
      this.resolvedPartId.set(partId);
      const partDto = this.parts().find((p) => p.id === partId) ?? null;
      this.resolvedPart.set(partDto);
      this.phase1Confirmed.set(true);
      this.autoGenerateJobs();
    } else {
      const dto: CreatePartRequestDto = {
        partName: controls.newPartName.value.trim(),
        partNumber: controls.newPartNumber.value.trim(),
        approxPartCycleTime: controls.newPartCycleTime.value.trim(),
        checkPerPart: Number(controls.newPartCheckPerPart.value)
      };

      this.partsApi.create(dto).subscribe({
        next: (created) => {
          this.resolvedPartId.set(created.id);
          // Build a minimal PartDto from the create form so barCycleTime can be computed
          const partDto: PartDto = {
            id: created.id,
            partName: dto.partName,
            partNumber: dto.partNumber,
            approxPartCycleTime: dto.approxPartCycleTime,
            checkPerPart: dto.checkPerPart
          };
          this.resolvedPart.set(partDto);
          this.partsApi.listActive().subscribe({ next: (data) => this.parts.set(data) });
          this.phase1Confirmed.set(true);
          this.autoGenerateJobs();
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

      while (this.jobsArray.length > 0) {
        this.jobsArray.removeAt(0);
      }
    }
    this.resolvedPart.set(null);
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

  protected recalculateBarCycleTime(index: number): void {
    const group = this.getJobGroup(index);
    const eppb = Number(group.get('estimatedPartsPerBar')?.value) || 0;
    const loader = String(group.get('barLoaderTime')?.value || '00:00:00');
    const result = this.calcBarCycleTime(
      this.resolvedPart()?.approxPartCycleTime ?? null,
      eppb,
      loader
    );
    group.patchValue({ barCycleTime: result }, { emitEvent: false });
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
      stockLotId: j.stockLotId !== null && j.stockLotId !== '' ? Number(j.stockLotId) : null,
      machineId: Number(j.machineId),
      partAmountPlanned: Number(j.partAmountPlanned),
      barAmountPlanned: Number(j.barAmountPlanned),
      barCycleTime: String(j.barCycleTime).trim(),
      estimatedPartsPerBar:
        j.estimatedPartsPerBar !== null && j.estimatedPartsPerBar !== ''
          ? Number(j.estimatedPartsPerBar)
          : null,
      dueDate: String(j.dueDate)
      // barLoaderTime intentionally excluded — form-only field
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
    this.resolvedPart.set(null);

    this.phase1Form.reset({
      customerId: 0,
      partMode: 'existing',
      partId: 0,
      newPartName: '',
      newPartNumber: '',
      newPartCycleTime: '00:00:30',
      newPartCheckPerPart: 1,
      partAmountRequested: 1,
      partsPerBar: 1,
      numberOfJobs: 1
    });

    while (this.jobsArray.length > 0) {
      this.jobsArray.removeAt(0);
    }
  }

  // Auto-generate job rows when Phase 1 is confirmed
  private autoGenerateJobs(): void {
    while (this.jobsArray.length > 0) {
      this.jobsArray.removeAt(0);
    }

    const p1 = this.phase1Form.getRawValue();
    const numJobs = Number(p1.numberOfJobs) || 1;
    const partsReq = Number(p1.partAmountRequested) || 0;
    const ppb = Number(p1.partsPerBar) || 1;
    const estBars = Math.ceil(partsReq / ppb);
    const partAmountPlanned = Math.ceil(partsReq / numJobs);
    const barAmountPlanned = Math.ceil(estBars / numJobs);
    const barCycleTime = this.calcBarCycleTime(
      this.resolvedPart()?.approxPartCycleTime ?? null,
      ppb,
      '00:00:00'
    );

    for (let i = 0; i < numJobs; i++) {
      this.jobsArray.push(
        this.createJobGroup(partAmountPlanned, barAmountPlanned, ppb, barCycleTime)
      );
    }
  }

  private calcBarCycleTime(
    approxCycleTime: string | null,
    estimatedPartsPerBar: number,
    barLoaderTime: string
  ): string {
    const parseHms = (hms: string): number => {
      const [h, m, s] = hms.split(':').map(Number);
      return (h || 0) * 3600 + (m || 0) * 60 + (s || 0);
    };
    const formatHms = (totalSeconds: number): string => {
      const h = Math.floor(totalSeconds / 3600);
      const m = Math.floor((totalSeconds % 3600) / 60);
      const s = Math.floor(totalSeconds % 60);
      return [h, m, s].map((n) => String(n).padStart(2, '0')).join(':');
    };
    const cycleSecs = approxCycleTime ? parseHms(approxCycleTime) : 0;
    const loaderSecs = parseHms(barLoaderTime);
    return formatHms(cycleSecs * estimatedPartsPerBar + loaderSecs);
  }

  private createJobGroup(
    partAmountPlanned = 0,
    barAmountPlanned = 0,
    estimatedPartsPerBar: number | null = null,
    barCycleTime = '00:00:00'
  ): FormGroup {
    return this.fb.nonNullable.group({
      stockLotId: [null as number | null],
      machineId: ['', [Validators.required, Validators.min(1)]],
      partAmountPlanned: [partAmountPlanned, [Validators.required, Validators.min(0)]],
      barAmountPlanned: [barAmountPlanned, [Validators.required, Validators.min(0)]],
      barCycleTime: [barCycleTime, [Validators.required, Validators.pattern(/^\d{2}:\d{2}:\d{2}$/)]],
      barLoaderTime: ['00:00:00', [Validators.pattern(/^\d{2}:\d{2}:\d{2}$/)]],
      estimatedPartsPerBar: [estimatedPartsPerBar],
      dueDate: ['', [Validators.required]]
    });
  }
}
