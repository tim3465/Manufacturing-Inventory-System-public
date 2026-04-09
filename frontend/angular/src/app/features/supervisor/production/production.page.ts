import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { debounceTime } from 'rxjs/operators';
import { JobsApi } from '../../../core/api/jobs.api';
import { OrdersApi } from '../../../core/api/orders.api';
import { ShiftsApi } from '../../../core/api/shifts.api';
import { OrderProductionSearchRequestDto } from '../../../core/dtos/orders/order-production-search-request.dto';
import { OrderProductionSearchResultDto } from '../../../core/dtos/orders/order-production-search-result.dto';
import { JobProductionDto } from '../../../core/dtos/jobs/job-production.dto';
import { JobProductionSearchRequestDto } from '../../../core/dtos/jobs/job-production-search-request.dto';
import { JobProductionSearchResultDto } from '../../../core/dtos/jobs/job-production-search-result.dto';
import { ShiftDto } from '../../../core/dtos/shifts/shift.dto';
import { ShiftProductionSearchRequestDto } from '../../../core/dtos/shifts/shift-production-search-request.dto';
import { ShiftProductionSearchResultDto } from '../../../core/dtos/shifts/shift-production-search-result.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../../core/ui/smart-table';
import { AddPartModalComponent } from './add-part-modal/add-part-modal.component';
import { AssignStockLotModalComponent } from './assign-stock-lot-modal/assign-stock-lot-modal.component';
import { ProductionPartsTabComponent } from './tabs/production-parts-tab/production-parts-tab.component';

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
  status: 'In Progress' | 'Completed';
  expanded: boolean;
  shifts: ShiftDto[];
  stockLotId: number | null;
  lotNumber: string | null;
}

@Component({
  selector: 'app-production-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PagerComponent, AddPartModalComponent, AssignStockLotModalComponent, ProductionPartsTabComponent],
  templateUrl: './production.page.html',
  styleUrl: './production.page.css'
})
export class ProductionPageComponent implements OnInit {
  private readonly ordersApi = inject(OrdersApi);
  private readonly jobsApi = inject(JobsApi);
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

  // ─── Jobs tab — smart table state ─────────────────────────────────────────
  protected readonly jobsTable = new SmartTableState({
    defaultSortColumn: 'DueDate',
    defaultSortDirection: 'asc',
    pageSize: 10
  });

  protected readonly jobsFilterForm = this.fb.nonNullable.group({
    dueDateFrom: [''],
    dueDateTo: [''],
    orderNumber: [''],
    partName: [''],
    partNumber: [''],
    machineName: [''],
    lotNumber: [''],
    status: ['']
  });

  private readonly jobsSearchResult = signal<JobProductionSearchResultDto | null>(null);

  private readonly jobStatusFilter = signal<string>('');

  protected readonly jobRows = computed<JobProductionRow[]>(() => {
    const items = (this.jobsSearchResult()?.items ?? []).map((j) => ({
      id: j.id,
      orderId: j.orderId,
      dueDate: j.dueDate,
      partName: j.partName,
      partNumber: j.partNumber,
      machineName: j.machineName,
      partAmountPlanned: j.partAmountPlanned,
      partsCompleted: j.partsCompleted,
      percentComplete: j.percentComplete,
      status: j.percentComplete >= 100 ? 'Completed' as const : 'In Progress' as const,
      expanded: false,
      shifts: j.shifts,
      stockLotId: j.stockLotId,
      lotNumber: j.lotNumber
    }));

    const statusFilter = this.jobStatusFilter();
    if (!statusFilter) return items;
    return items.filter(row => row.status === statusFilter);
  });
  protected readonly jobsTotalCount = computed(() => this.jobsSearchResult()?.totalCount ?? 0);
  protected readonly jobsTotalPages = computed(() => Math.ceil(this.jobsTotalCount() / this.jobsTable.pageSize()) || 1);

  // ─── Shifts tab — smart table state ───────────────────────────────────────
  protected readonly shiftsTable = new SmartTableState({
    defaultSortColumn: 'StartTime',
    defaultSortDirection: 'desc',
    pageSize: 10
  });

  protected readonly shiftsFilterForm = this.fb.nonNullable.group({
    operatorName: [''],
    jobNumber: [''],
    startTimeFrom: [''],
    startTimeTo: [''],
    stopTimeFrom: [''],
    stopTimeTo: ['']
  });

  private readonly shiftsSearchResult = signal<ShiftProductionSearchResultDto | null>(null);

  protected readonly shiftRows = computed(() => this.shiftsSearchResult()?.items ?? []);
  protected readonly shiftsTotalCount = computed(() => this.shiftsSearchResult()?.totalCount ?? 0);
  protected readonly shiftsTotalPages = computed(() => Math.ceil(this.shiftsTotalCount() / this.shiftsTable.pageSize()) || 1);

  protected readonly pageSizes = [5, 10, 25, 100];

  protected readonly isAddPartOpen = signal(false);

  @ViewChild(ProductionPartsTabComponent) partsTab?: ProductionPartsTabComponent;
  protected readonly selectedJobForLot = signal<JobProductionRow | null>(null);

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

    // Jobs filter form changes → reset page + search (only when jobs tab is active)
    this.jobsFilterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      if (this.selectedTab() !== 'jobs') return;
      this.jobStatusFilter.set(this.jobsFilterForm.getRawValue().status);
      this.jobsTable.resetPage();
      this.executeJobsSearch();
    });

    // Jobs sort, page, and page-size changes → search (guard: only when jobs tab active)
    effect(() => {
      this.jobsTable.sortColumn();
      this.jobsTable.sortDirection();
      this.jobsTable.currentPage();
      this.jobsTable.pageSize();

      untracked(() => {
        if (this.selectedTab() === 'jobs') {
          this.executeJobsSearch();
        }
      });
    });

    // Shifts filter form changes → reset page + search (only when shifts tab is active)
    this.shiftsFilterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      if (this.selectedTab() !== 'shifts') return;
      this.shiftsTable.resetPage();
      this.executeShiftsSearch();
    });

    // Shifts sort, page, and page-size changes → search (guard: only when shifts tab active)
    effect(() => {
      this.shiftsTable.sortColumn();
      this.shiftsTable.sortDirection();
      this.shiftsTable.currentPage();
      this.shiftsTable.pageSize();

      untracked(() => {
        if (this.selectedTab() === 'shifts') {
          this.executeShiftsSearch();
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
    if (tab === 'jobs' && !this.jobsTable.loading()) {
      this.executeJobsSearch();
    }
    if (tab === 'shifts' && !this.shiftsTable.loading()) {
      this.executeShiftsSearch();
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

  protected executeJobsSearch(): void {
    this.jobsTable.loading.set(true);
    this.jobsTable.error.set(null);

    // Collapse all expanded rows on each search
    this.expandedJobIds.set(new Set());

    const f = this.jobsFilterForm.getRawValue();

    const request: JobProductionSearchRequestDto = {
      sortColumn: this.jobsTable.sortColumn(),
      sortDirection: this.jobsTable.sortDirection(),
      page: this.jobsTable.currentPage(),
      pageSize: this.jobsTable.pageSize()
    };

    if (f.dueDateFrom?.trim()) {
      request.dueDateFrom = f.dueDateFrom.trim();
    }
    if (f.dueDateTo?.trim()) {
      request.dueDateTo = f.dueDateTo.trim();
    }
    if (f.orderNumber?.trim()) {
      request.orderNumber = f.orderNumber.trim();
    }
    if (f.partName?.trim()) {
      request.partName = f.partName.trim();
    }
    if (f.partNumber?.trim()) {
      request.partNumber = f.partNumber.trim();
    }
    if (f.machineName?.trim()) {
      request.machineName = f.machineName.trim();
    }
    if (f.lotNumber?.trim()) {
      request.lotNumber = f.lotNumber.trim();
    }

    this.jobsApi.searchProduction(request).subscribe({
      next: (result) => {
        this.jobsSearchResult.set(result);
        this.jobsTable.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load jobs';
        this.jobsTable.error.set(message);
        this.toast.error(message);
        this.jobsTable.loading.set(false);
      }
    });
  }

  protected executeShiftsSearch(): void {
    this.shiftsTable.loading.set(true);
    this.shiftsTable.error.set(null);

    const f = this.shiftsFilterForm.getRawValue();

    const request: ShiftProductionSearchRequestDto = {
      sortColumn: this.shiftsTable.sortColumn(),
      sortDirection: this.shiftsTable.sortDirection(),
      page: this.shiftsTable.currentPage(),
      pageSize: this.shiftsTable.pageSize()
    };

    if (f.operatorName?.trim()) {
      request.operatorName = f.operatorName.trim();
    }
    if (f.jobNumber?.trim()) {
      request.jobNumber = f.jobNumber.trim();
    }
    if (f.startTimeFrom?.trim()) {
      request.startTimeFrom = f.startTimeFrom.trim();
    }
    if (f.startTimeTo?.trim()) {
      request.startTimeTo = f.startTimeTo.trim();
    }
    if (f.stopTimeFrom?.trim()) {
      request.stopTimeFrom = f.stopTimeFrom.trim();
    }
    if (f.stopTimeTo?.trim()) {
      request.stopTimeTo = f.stopTimeTo.trim();
    }

    this.shiftsApi.searchProduction(request).subscribe({
      next: (result) => {
        this.shiftsSearchResult.set(result);
        this.shiftsTable.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load shifts';
        this.shiftsTable.error.set(message);
        this.toast.error(message);
        this.shiftsTable.loading.set(false);
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
    this.partsTab?.refresh();
  }

  protected openAssignLotModal(job: JobProductionRow): void {
    this.selectedJobForLot.set(job);
  }

  protected onLotAssigned(): void {
    this.selectedJobForLot.set(null);
    this.executeJobsSearch();
  }

  protected viewJobReport(jobId: number): void {
    this.router.navigate(['/supervisor/job-report', jobId]);
  }

  protected formatStopTime(stopTime: string | null): string {
    if (!stopTime) return 'In Progress';
    return new Date(stopTime).toLocaleString();
  }

  protected formatStartTime(startTime: string): string {
    return new Date(startTime).toLocaleString();
  }

}
