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

type Tab = 'orders' | 'jobs' | 'parts' | 'shifts';

interface TabDef {
  id: Tab;
  label: string;
}

interface JobRow {
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
}

@Component({
  selector: 'app-production-page',
  standalone: true,
  imports: [CommonModule, AddPartModalComponent],
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

  protected readonly jobRows = computed<JobRow[]>(() =>
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
        shifts: j.shifts
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
