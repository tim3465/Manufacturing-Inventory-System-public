import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { debounceTime } from 'rxjs/operators';
import { JobsApi } from '../../../core/api/jobs.api';
import { OrdersApi } from '../../../core/api/orders.api';
import { PartsApi } from '../../../core/api/parts.api';
import { ShiftsApi } from '../../../core/api/shifts.api';
import { OrderProductionSearchRequestDto } from '../../../core/dtos/orders/order-production-search-request.dto';
import { OrderProductionSearchResultDto } from '../../../core/dtos/orders/order-production-search-result.dto';
import { JobProductionDto } from '../../../core/dtos/jobs/job-production.dto';
import { PartSearchRequestDto } from '../../../core/dtos/parts/part-search-request.dto';
import { PartSearchResultDto } from '../../../core/dtos/parts/part-search-result.dto';
import { ShiftDto } from '../../../core/dtos/shifts/shift.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../../core/ui/smart-table';
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
  imports: [CommonModule, ReactiveFormsModule, PagerComponent, AddPartModalComponent, AssignStockLotModalComponent],
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
  private readonly fb = inject(FormBuilder);

  protected readonly selectedTab = signal<Tab>('orders');

  protected readonly tabs: TabDef[] = [
    { id: 'orders', label: 'Orders' },
    { id: 'jobs',   label: 'Jobs' },
    { id: 'parts',  label: 'Parts' },
    { id: 'shifts', label: 'Shifts' }
  ];

  // ─── Orders tab — smart table state ────────────────────────────────────────
  protected readonly ordersTable = new SmartTableState({
    defaultSortColumn: 'CustomerName',
    defaultSortDirection: 'asc',
    pageSize: 10
  });

  protected readonly ordersFilterForm = this.fb.nonNullable.group({
    customerName: [''],
    partName: [''],
    partNumber: ['']
  });

  private readonly ordersSearchResult = signal<OrderProductionSearchResultDto | null>(null);

  protected readonly orderRows = computed(() => this.ordersSearchResult()?.items ?? []);
  protected readonly ordersTotalCount = computed(() => this.ordersSearchResult()?.totalCount ?? 0);
  protected readonly ordersTotalPages = computed(() => Math.ceil(this.ordersTotalCount() / this.ordersTable.pageSize()) || 1);

  // Order expansion state (preserved from original)
  protected readonly expandedOrderIds = signal<Set<number>>(new Set());
  protected readonly orderJobsMap = signal<Map<number, JobProductionDto[]>>(new Map());
  protected readonly loadingOrderJobs = signal<Set<number>>(new Set());

  // ─── Jobs tab ──────────────────────────────────────────────────────────────
  protected readonly loadingJobs = signal(false);
  protected readonly loadingShifts = signal(false);

  protected readonly jobs = signal<JobProductionDto[]>([]);
  protected readonly shifts = signal<ShiftDto[]>([]);

  // ─── Parts tab — smart table state ────────────────────────────────────────
  protected readonly partsTable = new SmartTableState({
    defaultSortColumn: 'PartName',
    defaultSortDirection: 'asc',
    pageSize: 10
  });

  protected readonly pageSizes = [5, 10, 25, 100];

  protected readonly partsFilterForm = this.fb.nonNullable.group({
    partName: [''],
    partNumber: ['']
  });

  private readonly partsSearchResult = signal<PartSearchResultDto | null>(null);

  protected readonly partRows = computed(() => this.partsSearchResult()?.items ?? []);
  protected readonly partsTotalCount = computed(() => this.partsSearchResult()?.totalCount ?? 0);
  protected readonly partsTotalPages = computed(() => Math.ceil(this.partsTotalCount() / this.partsTable.pageSize()) || 1);

  protected readonly isAddPartOpen = signal(false);
  protected readonly selectedJobForLot = signal<JobProductionRow | null>(null);

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

  constructor() {
    // Orders filter form changes → reset page + search (only when orders tab is active)
    this.ordersFilterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      if (this.selectedTab() !== 'orders') return;
      this.ordersTable.resetPage();
      this.executeOrdersSearch();
    });

    // Orders sort, page, and page-size changes → search (guard: only when orders tab active)
    effect(() => {
      this.ordersTable.sortColumn();
      this.ordersTable.sortDirection();
      this.ordersTable.currentPage();
      this.ordersTable.pageSize();

      untracked(() => {
        if (this.selectedTab() === 'orders') {
          this.executeOrdersSearch();
        }
      });
    });

    // Parts filter form changes → reset page + search (only when parts tab is active)
    this.partsFilterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      if (this.selectedTab() !== 'parts') return;
      this.partsTable.resetPage();
      this.executePartsSearch();
    });

    // Sort, page, and page-size changes → search (guard: only when parts tab active)
    effect(() => {
      this.partsTable.sortColumn();
      this.partsTable.sortDirection();
      this.partsTable.currentPage();
      this.partsTable.pageSize();

      untracked(() => {
        if (this.selectedTab() === 'parts') {
          this.executePartsSearch();
        }
      });
    });
  }

  ngOnInit(): void {
    this.executeOrdersSearch();
  }

  protected selectTab(tab: Tab): void {
    this.selectedTab.set(tab);
    if (tab === 'orders' && !this.ordersTable.loading()) {
      this.executeOrdersSearch();
    }
    if (tab === 'jobs' && this.jobs().length === 0 && !this.loadingJobs()) {
      this.loadJobs();
    }
    if (tab === 'parts' && !this.partsTable.loading()) {
      this.executePartsSearch();
    }
    if (tab === 'shifts' && this.shifts().length === 0 && !this.loadingShifts()) {
      this.loadShifts();
    }
  }

  protected executeOrdersSearch(): void {
    this.ordersTable.loading.set(true);
    this.ordersTable.error.set(null);

    // Collapse all expanded rows on each search
    this.expandedOrderIds.set(new Set());

    const f = this.ordersFilterForm.getRawValue();

    const request: OrderProductionSearchRequestDto = {
      sortColumn: this.ordersTable.sortColumn(),
      sortDirection: this.ordersTable.sortDirection(),
      page: this.ordersTable.currentPage(),
      pageSize: this.ordersTable.pageSize()
    };

    if (f.customerName?.trim()) {
      request.customerName = f.customerName.trim();
    }
    if (f.partName?.trim()) {
      request.partName = f.partName.trim();
    }
    if (f.partNumber?.trim()) {
      request.partNumber = f.partNumber.trim();
    }

    this.ordersApi.searchProduction(request).subscribe({
      next: (result) => {
        this.ordersSearchResult.set(result);
        this.ordersTable.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load orders';
        this.ordersTable.error.set(message);
        this.toast.error(message);
        this.ordersTable.loading.set(false);
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

  protected executePartsSearch(): void {
    this.partsTable.loading.set(true);
    this.partsTable.error.set(null);

    const f = this.partsFilterForm.getRawValue();

    const request: PartSearchRequestDto = {
      sortColumn: this.partsTable.sortColumn(),
      sortDirection: this.partsTable.sortDirection(),
      page: this.partsTable.currentPage(),
      pageSize: this.partsTable.pageSize()
    };

    if (f.partName?.trim()) {
      request.partName = f.partName.trim();
    }
    if (f.partNumber?.trim()) {
      request.partNumber = f.partNumber.trim();
    }

    this.partsApi.search(request).subscribe({
      next: (result) => {
        this.partsSearchResult.set(result);
        this.partsTable.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load parts';
        this.partsTable.error.set(message);
        this.toast.error(message);
        this.partsTable.loading.set(false);
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
    this.executePartsSearch();
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
