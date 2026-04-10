import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { debounceTime } from 'rxjs/operators';
import { JobsApi } from '../../core/api/jobs.api';
import { JobShiftDto } from '../../core/dtos/jobs/job-shift.dto';
import { MyJobSearchRequestDto } from '../../core/dtos/jobs/my-job-search-request.dto';
import { MyJobSearchResultDto } from '../../core/dtos/jobs/my-job-search-result.dto';
import { ToastService } from '../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../core/ui/smart-table';

interface JobRow {
  id: number;
  jobNumber: string;
  partNumber: string;
  machineName: string;
  status: 'In Progress' | 'Completed';
}

@Component({
  selector: 'app-my-jobs-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PagerComponent],
  templateUrl: './my-jobs.page.html',
  styleUrl: './my-jobs.page.css'
})
export class MyJobsPageComponent {
  private readonly jobsApi = inject(JobsApi);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  protected readonly table = new SmartTableState({
    defaultSortColumn: 'JobNumber',
    defaultSortDirection: 'asc',
    pageSize: 10
  });

  protected readonly pageSizes = [5, 10, 25, 100];

  protected readonly filterForm = this.fb.nonNullable.group({
    jobNumber: [''],
    partNumber: [''],
    machineName: [''],
    status: ['']
  });

  private readonly searchResult = signal<MyJobSearchResultDto | null>(null);

  protected readonly rows = computed<JobRow[]>(() =>
    (this.searchResult()?.items ?? []).map(j => ({
      id: j.id,
      jobNumber: j.jobNumber,
      partNumber: j.partNumber,
      machineName: j.machineName,
      status: j.endedDateTime ? 'Completed' as const : 'In Progress' as const
    }))
  );

  protected readonly totalCount = computed(() => this.searchResult()?.totalCount ?? 0);
  protected readonly totalPages = computed(() => Math.ceil(this.totalCount() / this.table.pageSize()) || 1);

  protected readonly expandedJobId = signal<number | null>(null);
  private readonly shiftCache = signal<Map<number, JobShiftDto[]>>(new Map());
  protected readonly loadingShiftForJobId = signal<number | null>(null);

  constructor() {
    this.filterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.table.resetPage();
      this.executeSearch();
    });

    effect(() => {
      this.table.sortColumn();
      this.table.sortDirection();
      this.table.currentPage();
      this.table.pageSize();

      untracked(() => this.executeSearch());
    });
  }

  protected executeSearch(): void {
    this.table.loading.set(true);
    this.table.error.set(null);
    this.expandedJobId.set(null);

    const f = this.filterForm.getRawValue();

    const request: MyJobSearchRequestDto = {
      sortColumn: this.table.sortColumn(),
      sortDirection: this.table.sortDirection(),
      page: this.table.currentPage(),
      pageSize: this.table.pageSize()
    };

    if (f.jobNumber?.trim()) {
      request.jobNumber = f.jobNumber.trim();
    }
    if (f.partNumber?.trim()) {
      request.partNumber = f.partNumber.trim();
    }
    if (f.machineName?.trim()) {
      request.machineName = f.machineName.trim();
    }
    if (f.status) {
      request.status = f.status;
    }

    this.jobsApi.searchMyJobs(request).subscribe({
      next: (result) => {
        this.searchResult.set(result);
        this.table.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load jobs';
        this.table.error.set(message);
        this.toast.error(message);
        this.table.loading.set(false);
      }
    });
  }

  protected toggleJob(id: number): void {
    if (this.expandedJobId() === id) {
      this.expandedJobId.set(null);
      return;
    }

    this.expandedJobId.set(id);

    if (!this.shiftCache().has(id)) {
      this.loadingShiftForJobId.set(id);

      this.jobsApi.getMyJobShifts(id).subscribe({
        next: (shifts) => {
          const updated = new Map(this.shiftCache());
          updated.set(id, shifts);
          this.shiftCache.set(updated);
          this.loadingShiftForJobId.set(null);
        },
        error: () => {
          this.toast.error('Failed to load shifts');
          this.loadingShiftForJobId.set(null);
        }
      });
    }
  }

  protected shiftsForJob(id: number): JobShiftDto[] {
    return this.shiftCache().get(id) ?? [];
  }

  protected hasShiftCache(id: number): boolean {
    return this.shiftCache().has(id);
  }

  protected viewJobReport(jobId: number, event: Event): void {
    event.stopPropagation();
    this.router.navigate(['/machinist/job-report', jobId]);
  }
}
