import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { JobsApi } from '../../core/api/jobs.api';
import { MyJobDto } from '../../core/dtos/jobs/my-job.dto';
import { ToastService } from '../../core/ui/toast/toast.service';

interface ShiftRow {
  id: number;
  operatorName: string;
  startTime: string;
  stopTime: string | null;
  status: 'In Progress' | 'Completed';
  statusIcon: 'play' | 'check';
}

interface JobRow {
  id: number;
  jobNumber: string;
  partNumber: string;
  machineName: string;
  status: 'In Progress' | 'Not Running' | 'Completed';
  shifts: ShiftRow[];
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
  protected readonly showTimestamps = signal(false);

  protected readonly jobRows = computed<JobRow[]>(() =>
    this.jobs().map(j => {
      const shifts: ShiftRow[] = j.shifts.map(s => ({
        id: s.id,
        operatorName: s.operatorName,
        startTime: s.startTime,
        stopTime: s.stopTime,
        status: s.stopTime === null ? 'In Progress' as const : 'Completed' as const,
        statusIcon: s.stopTime === null ? 'play' as const : 'check' as const
      }));

      let status: 'In Progress' | 'Not Running' | 'Completed';
      if (j.endedDateTime) {
        status = 'Completed';
      } else if (shifts.length > 0 && shifts.every(s => s.stopTime !== null)) {
        status = 'Not Running';
      } else {
        status = 'In Progress';
      }

      return {
        id: j.id,
        jobNumber: j.jobNumber,
        partNumber: j.partNumber,
        machineName: j.machineName,
        status,
        shifts
      };
    })
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

  protected toggleTimestamps(): void {
    this.showTimestamps.update(v => !v);
  }
}
