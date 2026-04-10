import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { ShiftIssueLogsApi } from '../../../../core/api/shift-issue-logs.api';
import { ShiftIssueLogDto } from '../../../../core/dtos/shift-issue-logs/shift-issue-log.dto';
import { ISSUE_TYPE_LABELS, IssueType } from '../../../../core/dtos/shift-issue-logs/create-shift-issue-log-request.dto';
import { LogIssueFormComponent } from '../log-issue-form/log-issue-form.component';
import { AuthService } from '../../../../core/auth/auth.service';

interface IssueLogRow {
  id: number;
  issueTypeLabel: string;
  scrapQuantity: number;
  downtimeHours: number;
  downtimeMinutes: number;
  description: string;
  createdDateTime: string;
  createdByUserId: number | null;
  createdByUserDisplayName: string;
}

@Component({
  selector: 'app-issue-logs-panel',
  standalone: true,
  imports: [LogIssueFormComponent],
  templateUrl: './issue-logs-panel.component.html'
})
export class IssueLogsPanelComponent implements OnInit {
  private readonly issueLogsApi = inject(ShiftIssueLogsApi);
  private readonly auth = inject(AuthService);

  @Input({ required: true }) shiftId!: number;
  @Output() closed = new EventEmitter<void>();
  @Output() issueCreated = new EventEmitter<void>();

  protected readonly loading = signal(true);
  protected readonly issueLogs = signal<IssueLogRow[]>([]);
  protected readonly currentUserId = this.auth.getUserId();

  ngOnInit(): void {
    this.loadLogs();
  }

  protected loadLogs(): void {
    this.loading.set(true);
    this.issueLogsApi.getByShift(this.shiftId).subscribe({
      next: (dtos) => {
        this.issueLogs.set(dtos.map((d) => this.toRow(d)));
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  protected onIssueSubmitted(): void {
    this.loadLogs();
    this.issueCreated.emit();
  }

  protected onClose(): void {
    this.closed.emit();
  }

  private toRow(dto: ShiftIssueLogDto): IssueLogRow {
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
      issueTypeLabel: ISSUE_TYPE_LABELS[dto.issueType as IssueType] ?? 'Unknown',
      scrapQuantity: dto.scrapQuantity,
      downtimeHours,
      downtimeMinutes,
      description: dto.description,
      createdDateTime: formatted,
      createdByUserId: dto.createdByUserId,
      createdByUserDisplayName: dto.createdByUserDisplayName
    };
  }
}
