import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { JobsApi } from '../../../core/api/jobs.api';
import { JobReportDto, JobReportIssueLogDto } from '../../../core/dtos/jobs/job-report.dto';
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

  protected readonly chartData = computed(() => {
    const r = this.report();
    if (!r) return null;

    const partsMade = r.totalPartsMade;
    const partsPlanned = r.partAmountPlanned;
    const scrap = r.totalScrap;
    const partsMax = Math.max(partsMade, partsPlanned, scrap, 1);

    const barsConsumed = r.totalBarsConsumed;
    const barsPlanned = r.barAmountPlanned;
    const barsMax = Math.max(barsConsumed, barsPlanned, 1);

    const actualPpb = r.actualPartsPerBar ?? 0;
    const estimatedPpb = r.estimatedPartsPerBar ?? 0;
    const ppbMax = Math.max(actualPpb, estimatedPpb, 1);

    return {
      parts: {
        made: { value: partsMade, pct: (partsMade / partsMax) * 100 },
        planned: { value: partsPlanned, pct: (partsPlanned / partsMax) * 100 },
        scrap: { value: scrap, pct: (scrap / partsMax) * 100 }
      },
      bars: {
        consumed: { value: barsConsumed, pct: (barsConsumed / barsMax) * 100 },
        planned: { value: barsPlanned, pct: (barsPlanned / barsMax) * 100 }
      },
      ppb: {
        actual: { value: actualPpb, pct: (actualPpb / ppbMax) * 100 },
        estimated: { value: estimatedPpb, pct: (estimatedPpb / ppbMax) * 100 }
      }
    };
  });

  protected readonly issueLogRows = computed(() => {
    const r = this.report();
    if (!r || !r.issueLogs) return [];
    return r.issueLogs.map((log) => this.toIssueLogRow(log));
  });

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

  private toIssueLogRow(dto: JobReportIssueLogDto): IssueLogRow {
    const issueTypeLabels: Record<number, string> = { 1: 'Setup', 2: 'Production' };

    let downtimeHours = 0;
    let downtimeMinutes = 0;
    if (dto.downtime) {
      const parts = dto.downtime.split(':');
      downtimeHours = parseInt(parts[0], 10) || 0;
      downtimeMinutes = parseInt(parts[1], 10) || 0;
    }

    const date = new Date(dto.createdDateTime);
    const formatted = date.toLocaleString(undefined, {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: 'numeric',
      minute: '2-digit'
    });

    return {
      id: dto.id,
      operatorName: dto.operatorName,
      issueTypeLabel: issueTypeLabels[dto.issueType] ?? 'Unknown',
      scrapQuantity: dto.scrapQuantity,
      downtimeHours,
      downtimeMinutes,
      description: dto.description,
      createdDateTime: formatted
    };
  }
}

interface IssueLogRow {
  id: number;
  operatorName: string;
  issueTypeLabel: string;
  scrapQuantity: number;
  downtimeHours: number;
  downtimeMinutes: number;
  description: string;
  createdDateTime: string;
}
