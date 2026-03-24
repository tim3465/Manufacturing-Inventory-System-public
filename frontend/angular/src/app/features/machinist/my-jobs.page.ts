import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { JobsApi } from '../../core/api/jobs.api';
import { JobShiftDto } from '../../core/dtos/jobs/job-shift.dto';
import { MyJobListItemDto } from '../../core/dtos/jobs/my-job.dto';
import { ToastService } from '../../core/ui/toast/toast.service';

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
  imports: [CommonModule],
  templateUrl: './my-jobs.page.html',
  styleUrl: './my-jobs.page.css'
})
export class MyJobsPageComponent implements OnInit {
  private readonly jobsApi = inject(JobsApi);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly jobs = signal<MyJobListItemDto[]>([]);
  protected readonly expandedJobId = signal<number | null>(null);
  private readonly shiftCache = signal<Map<number, JobShiftDto[]>>(new Map());
  protected readonly loadingShiftForJobId = signal<number | null>(null);

  protected readonly jobRows = computed<JobRow[]>(() =>
    this.jobs().map(j => ({
      id: j.id,
      jobNumber: j.jobNumber,
      partNumber: j.partNumber,
      machineName: j.machineName,
      status: j.endedDateTime ? 'Completed' as const : 'In Progress' as const
    }))
  );

  ngOnInit(): void {
    this.jobsApi.listMyJobs().subscribe({
      next: (data) => {
        this.jobs.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load jobs.');
        this.toast.error('Failed to load jobs');
        this.loading.set(false);
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
}
