import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { debounceTime } from 'rxjs/operators';
import { JobsApi } from '../../../../../core/api/jobs.api';
import { OrdersApi } from '../../../../../core/api/orders.api';
import { JobProductionDto } from '../../../../../core/dtos/jobs/job-production.dto';
import { OrderProductionSearchRequestDto } from '../../../../../core/dtos/orders/order-production-search-request.dto';
import { OrderProductionSearchResultDto } from '../../../../../core/dtos/orders/order-production-search-result.dto';
import { ToastService } from '../../../../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../../../../core/ui/smart-table';

@Component({
  selector: 'app-production-orders-tab',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PagerComponent],
  templateUrl: './production-orders-tab.component.html',
  styleUrl: './production-orders-tab.component.css'
})
export class ProductionOrdersTabComponent implements OnInit {
  private readonly ordersApi = inject(OrdersApi);
  private readonly jobsApi = inject(JobsApi);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

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

  // Order expansion state
  protected readonly expandedOrderIds = signal<Set<number>>(new Set());
  protected readonly orderJobsMap = signal<Map<number, JobProductionDto[]>>(new Map());
  protected readonly loadingOrderJobs = signal<Set<number>>(new Set());

  protected readonly pageSizes = [5, 10, 25, 100];

  constructor() {
    this.ordersFilterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.ordersTable.resetPage();
      this.executeOrdersSearch();
    });

    effect(() => {
      this.ordersTable.sortColumn();
      this.ordersTable.sortDirection();
      this.ordersTable.currentPage();
      this.ordersTable.pageSize();

      untracked(() => {
        this.executeOrdersSearch();
      });
    });
  }

  ngOnInit(): void {
    this.executeOrdersSearch();
  }

  refresh(): void {
    this.executeOrdersSearch();
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

  protected clampPercent(value: number): number {
    return Math.min(100, Math.max(0, value));
  }

  protected getPieStrokeDasharray(percent: number): string {
    const clamped = this.clampPercent(percent);
    const circumference = 2 * Math.PI * 15.9155;
    const filled = (clamped / 100) * circumference;
    return `${filled} ${circumference - filled}`;
  }

  protected viewJobReport(jobId: number): void {
    this.router.navigate(['/supervisor/job-report', jobId]);
  }

  private executeOrdersSearch(): void {
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
}
