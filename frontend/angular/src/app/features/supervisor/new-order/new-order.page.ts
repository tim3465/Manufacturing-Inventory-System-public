import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

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
import { Router } from '@angular/router';
import { CustomersApi } from '../../../core/api/customers.api';
import { OrderPlanningApi } from '../../../core/api/order-planning.api';
import { PartsApi } from '../../../core/api/parts.api';
import { CustomerDto } from '../../../core/dtos/customers/customer.dto';
import { CreateJobInOrderRequestDto, CreateOrderWithJobsRequestDto } from '../../../core/dtos/order-planning/order-planning.dto';
import { PartDto } from '../../../core/dtos/parts/part.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';

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
  private readonly orderPlanningApi = inject(OrderPlanningApi);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly customers = signal<CustomerDto[]>([]);
  protected readonly parts = signal<PartDto[]>([]);
  protected readonly submitting = signal(false);
  protected readonly successOrderId = signal<number | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    customerId: [0, [Validators.required, Validators.min(1)]],
    partId: [0, [Validators.required, Validators.min(1)]],
    partAmountRequested: [1, [Validators.required, Validators.min(1)]],
    partsPerBar: [1, [Validators.required, Validators.min(1)]],
    jobs: this.fb.array([this.createJobGroup()])
  });

  get jobsArray(): FormArray {
    return this.form.get('jobs') as FormArray;
  }

  getJobGroup(index: number): FormGroup {
    return this.jobsArray.at(index) as FormGroup;
  }

  getControl(group: AbstractControl, name: string) {
    return (group as FormGroup).get(name);
  }

  ngOnInit(): void {
    this.customersApi.listActive().subscribe({
      next: (customers) => this.customers.set(customers),
      error: () => this.toast.error('Failed to load customers')
    });

    this.partsApi.listActive().subscribe({
      next: (parts) => this.parts.set(parts),
      error: () => this.toast.error('Failed to load parts')
    });
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

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();

    const jobs: CreateJobInOrderRequestDto[] = (raw.jobs as unknown as RawJobValue[]).map((j) => ({
      stockLotId: Number(j['stockLotId']),
      machineId: Number(j['machineId']),
      partAmountPlanned: Number(j['partAmountPlanned']),
      barAmountPlanned: Number(j['barAmountPlanned']),
      barCycleTime: String(j['barCycleTime']).trim(),
      barsInJob: Number(j['barsInJob']),
      estimatedPartsPerBar: j['estimatedPartsPerBar'] !== null && j['estimatedPartsPerBar'] !== '' ? Number(j['estimatedPartsPerBar']) : null,
      dueDate: String(j['dueDate'])
    }));

    const dto: CreateOrderWithJobsRequestDto = {
      customerId: Number(raw.customerId),
      partId: Number(raw.partId),
      partAmountRequested: Number(raw.partAmountRequested),
      partsPerBar: Number(raw.partsPerBar),
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
    this.form.reset({
      customerId: 0,
      partId: 0,
      partAmountRequested: 1,
      partsPerBar: 1
    });
    while (this.jobsArray.length > 1) {
      this.jobsArray.removeAt(1);
    }
    this.jobsArray.at(0).reset({
      stockLotId: '',
      machineId: '',
      partAmountPlanned: '',
      barAmountPlanned: '',
      barCycleTime: '',
      barsInJob: '',
      estimatedPartsPerBar: '',
      dueDate: ''
    });
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
