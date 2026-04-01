import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { JobsApi } from '../../../core/api/jobs.api';
import { JobReportDto } from '../../../core/dtos/jobs/job-report.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-job-report-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './job-report.page.html',
  styleUrl: './job-report.page.css'
})
export class JobReportPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly jobsApi = inject(JobsApi);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal<boolean>(true);
  protected readonly report = signal<JobReportDto | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.toast.error('Invalid job ID');
      this.router.navigate(['/supervisor/production']);
      return;
    }

    this.jobsApi.getReport(id).subscribe({
      next: (data) => {
        this.report.set(data);
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err);
        this.loading.set(false);
      }
    });
  }

  protected formatDateTime(value: string | null): string {
    if (!value) return '\u2014';
    return new Date(value).toLocaleString();
  }

  protected formatDowntime(value: string | null): string {
    return value ?? '\u2014';
  }

  protected formatNumber(value: number | null): string {
    if (value === null || value === undefined) return '\u2014';
    return String(value);
  }
}
