import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { JobsApi } from '../../core/api/jobs.api';
import { MyJobDto } from '../../core/dtos/jobs/my-job.dto';
import { ShiftDto } from '../../core/dtos/shifts/shift.dto';
import { ToastService } from '../../core/ui/toast/toast.service';

interface JobRow {
  id: number;
  jobNumber: string;
  partNumber: string;
  machineName: string;
  status: 'In Progress' | 'Completed';
  shifts: ShiftDto[];
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
  protected readonly jobs = signal<MyJobDto[]>([]);
  protected readonly expandedJobId = signal<number | null>(null);

  protected readonly jobRows = computed<JobRow[]>(() =>
    this.jobs().map(j => ({
      id: j.id,
      jobNumber: j.jobNumber,
      partNumber: j.partNumber,
      machineName: j.machineName,
      status: j.endedDateTime ? 'Completed' as const : 'In Progress' as const,
      shifts: j.shifts
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
    this.expandedJobId.set(this.expandedJobId() === id ? null : id);
  }
}
