import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { JobsApi } from '../../../core/api/jobs.api';
import { OrdersApi } from '../../../core/api/orders.api';
import { PartsApi } from '../../../core/api/parts.api';
import { ShiftsApi } from '../../../core/api/shifts.api';
import { JobProductionDto } from '../../../core/dtos/jobs/job-production.dto';
import { OrderProductionDto } from '../../../core/dtos/orders/order-production.dto';
import { PartDto } from '../../../core/dtos/parts/part.dto';
import { ShiftDto } from '../../../core/dtos/shifts/shift.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { AddPartModalComponent } from './add-part-modal/add-part-modal.component';
import { AssignStockLotModalComponent } from './assign-stock-lot-modal/assign-stock-lot-modal.component';

type Tab = 'orders' | 'jobs' | 'parts' | 'shifts';

interface TabDef {
  id: Tab;
  label: string;
}

export interface JobProductionRow {
  id: number;
  orderId: number;
  dueDate: string;
  partName: string;
  partNumber: string;
  machineName: string;
  partAmountPlanned: number;
  partsCompleted: number;
  percentComplete: number;
  expanded: boolean;
  shifts: ShiftDto[];
  stockLotId: number | null;
  lotNumber: string | null;
}

@Component({
  selector: 'app-production-page',
  standalone: true,
  imports: [CommonModule, AddPartModalComponent, AssignStockLotModalComponent],
  templateUrl: './production.page.html',
  styleUrl: './production.page.css'
})
export class ProductionPageComponent implements OnInit {
  private readonly ordersApi = inject(OrdersApi);
  private readonly jobsApi = inject(JobsApi);
  private readonly partsApi = inject(PartsApi);
  private readonly shiftsApi = inject(ShiftsApi);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);

  protected readonly selectedTab = signal<Tab>('orders');

  protected readonly tabs: TabDef[] = [
    { id: 'orders', label: 'Orders' },
    { id: 'jobs',   label: 'Jobs' },
    { id: 'parts',  label: 'Parts' },
    { id: 'shifts', label: 'Shifts' }
  ];

  protected readonly loadingOrders = signal(true);
  protected readonly loadingJobs = signal(false);
  protected readonly loadingParts = signal(false);
  protected readonly loadingShifts = signal(false);

  protected readonly orders = signal<OrderProductionDto[]>([]);
  protected readonly jobs = signal<JobProductionDto[]>([]);
  protected readonly parts = signal<PartDto[]>([]);
  protected readonly shifts = signal<ShiftDto[]>([]);

  protected readonly isAddPartOpen = signal(false);
  protected readonly selectedJobForLot = signal<JobProductionRow | null>(null);

  // Order expansion state
  protected readonly expandedOrderIds = signal<Set<number>>(new Set());
  protected readonly orderJobsMap = signal<Map<number, JobProductionDto[]>>(new Map());
  protected readonly loadingOrderJobs = signal<Set<number>>(new Set());

  protected readonly jobRows = computed<JobProductionRow[]>(() =>
    this.jobs()
      .slice()
      .sort((a, b) => a.dueDate.localeCompare(b.dueDate))
      .map((j) => ({
        id: j.id,
        orderId: j.orderId,
        dueDate: j.dueDate,
        partName: j.partName,
        partNumber: j.partNumber,
        machineName: j.machineName,
        partAmountPlanned: j.partAmountPlanned,
        partsCompleted: j.partsCompleted,
        percentComplete: j.percentComplete,
        expanded: false,
        shifts: j.shifts,
        stockLotId: j.stockLotId,
        lotNumber: j.lotNumber
      }))
  );

  // Mutable copy of job rows so we can toggle expansion
  protected expandedJobIds = signal<Set<number>>(new Set());

  ngOnInit(): void {
    this.loadOrders();
  }

  protected selectTab(tab: Tab): void {
    this.selectedTab.set(tab);
    if (tab === 'jobs' && this.jobs().length === 0 && !this.loadingJobs()) {
      this.loadJobs();
    }
    if (tab === 'parts' && this.parts().length === 0 && !this.loadingParts()) {
      this.loadParts();
    }
    if (tab === 'shifts' && this.shifts().length === 0 && !this.loadingShifts()) {
      this.loadShifts();
    }
  }

  protected loadOrders(): void {
    this.loadingOrders.set(true);
    this.ordersApi.listProduction().subscribe({
      next: (data) => {
        this.orders.set(data);
        this.loadingOrders.set(false);
      },
      error: () => {
        this.toast.error('Failed to load orders');
        this.loadingOrders.set(false);
      }
    });
  }

  protected loadJobs(): void {
    this.loadingJobs.set(true);
    this.jobsApi.listProduction().subscribe({
      next: (data) => {
        this.jobs.set(data);
        this.loadingJobs.set(false);
      },
      error: () => {
        this.toast.error('Failed to load jobs');
        this.loadingJobs.set(false);
      }
    });
  }

  protected loadParts(): void {
    this.loadingParts.set(true);
    this.partsApi.listActive().subscribe({
      next: (data) => {
        this.parts.set(data);
        this.loadingParts.set(false);
      },
      error: () => {
        this.toast.error('Failed to load parts');
        this.loadingParts.set(false);
      }
    });
  }

  protected loadShifts(): void {
    this.loadingShifts.set(true);
    this.shiftsApi.listProduction().subscribe({
      next: (data) => {
        this.shifts.set(data);
        this.loadingShifts.set(false);
      },
      error: () => {
        this.toast.error('Failed to load shifts');
        this.loadingShifts.set(false);
      }
    });
  }

  protected toggleOrderExpanded(orderId: number): void {
    const current = new Set(this.expandedOrderIds());
    if (current.has(orderId)) {
      current.delete(orderId);
    } else {
      current.add(orderId);
      // Lazy-load jobs on first expand
      if (!this.orderJobsMap().has(orderId)) {
        const loading = new Set(this.loadingOrderJobs());
        loading.add(orderId);
        this.loadingOrderJobs.set(loading);

        this.jobsApi.listByOrder(orderId).subscribe({
          next: (jobs) => {
            const map = new Map(this.orderJobsMap());
            map.set(orderId, jobs);
            this.orderJobsMap.set(map);

            const done = new Set(this.loadingOrderJobs());
            done.delete(orderId);
            this.loadingOrderJobs.set(done);
          },
          error: () => {
            this.toast.error('Failed to load jobs for order');
            const done = new Set(this.loadingOrderJobs());
            done.delete(orderId);
            this.loadingOrderJobs.set(done);
          }
        });
      }
    }
    this.expandedOrderIds.set(current);
  }

  protected isOrderExpanded(orderId: number): boolean {
    return this.expandedOrderIds().has(orderId);
  }

  protected getOrderJobs(orderId: number): JobProductionDto[] {
    return this.orderJobsMap().get(orderId) ?? [];
  }

  protected isOrderJobsLoading(orderId: number): boolean {
    return this.loadingOrderJobs().has(orderId);
  }

  protected goToNewOrder(): void {
    this.router.navigate(['/supervisor/new-order']);
  }

  protected clampPercent(value: number): number {
    return Math.min(100, Math.max(0, value));
  }

  protected getPieStrokeDasharray(percent: number): string {
    const clamped = this.clampPercent(percent);
    const circumference = 2 * Math.PI * 15.9155;
    const filled = (clamped / 100) * circumference;
    return `${filled} ${circumference - filled}`;
  }

  protected toggleJobExpanded(jobId: number): void {
    const current = new Set(this.expandedJobIds());
    if (current.has(jobId)) {
      current.delete(jobId);
    } else {
      current.add(jobId);
    }
    this.expandedJobIds.set(current);
  }

  protected isJobExpanded(jobId: number): boolean {
    return this.expandedJobIds().has(jobId);
  }

  protected openAddPart(): void {
    this.isAddPartOpen.set(true);
  }

  protected onPartCreated(): void {
    this.parts.set([]);
    this.loadParts();
  }

  protected openAssignLotModal(job: JobProductionRow): void {
    this.selectedJobForLot.set(job);
  }

  protected onLotAssigned(): void {
    this.selectedJobForLot.set(null);
    this.loadJobs();
  }

  protected formatStopTime(stopTime: string | null): string {
    if (!stopTime) return 'In Progress';
    return new Date(stopTime).toLocaleString();
  }

  protected formatStartTime(startTime: string): string {
    return new Date(startTime).toLocaleString();
  }

  protected formatCycleTime(time: string): string {
    // approxPartCycleTime comes as HH:MM:SS string from TimeSpan
    return time ?? '—';
  }
}
