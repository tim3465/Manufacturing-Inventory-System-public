import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { debounceTime } from 'rxjs/operators';
import { JobsApi } from '../../../../../core/api/jobs.api';
import { JobProductionSearchRequestDto } from '../../../../../core/dtos/jobs/job-production-search-request.dto';
import { JobProductionSearchResultDto } from '../../../../../core/dtos/jobs/job-production-search-result.dto';
import { ShiftDto } from '../../../../../core/dtos/shifts/shift.dto';
import { ToastService } from '../../../../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../../../../core/ui/smart-table';

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
  selector: 'app-production-jobs-tab',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PagerComponent],
  templateUrl: './production-jobs-tab.component.html',
  styleUrl: './production-jobs-tab.component.css'
})
export class ProductionJobsTabComponent implements OnInit {
  private readonly jobsApi = inject(JobsApi);
  private readonly toast = inject(ToastService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  @Output() assignLot = new EventEmitter<JobProductionRow>();

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

  protected readonly pageSizes = [5, 10, 25, 100];

  protected expandedJobIds = signal<Set<number>>(new Set());

  constructor() {
    this.jobsFilterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.jobStatusFilter.set(this.jobsFilterForm.getRawValue().status);
      this.jobsTable.resetPage();
      this.executeJobsSearch();
    });

    effect(() => {
      this.jobsTable.sortColumn();
      this.jobsTable.sortDirection();
      this.jobsTable.currentPage();
      this.jobsTable.pageSize();

      untracked(() => {
        this.executeJobsSearch();
      });
    });
  }

  ngOnInit(): void {
    this.executeJobsSearch();
  }

  refresh(): void {
    this.executeJobsSearch();
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

  protected openAssignLotModal(job: JobProductionRow): void {
    this.assignLot.emit(job);
  }

  protected viewJobReport(jobId: number): void {
    this.router.navigate(['/supervisor/job-report', jobId]);
  }

  protected formatStartTime(startTime: string): string {
    return new Date(startTime).toLocaleString();
  }

  protected formatStopTime(stopTime: string | null): string {
    if (!stopTime) return 'In Progress';
    return new Date(stopTime).toLocaleString();
  }

  private executeJobsSearch(): void {
    this.jobsTable.loading.set(true);
    this.jobsTable.error.set(null);

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
}
