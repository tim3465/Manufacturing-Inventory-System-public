import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ShiftIssueLogsApi } from '../../../../core/api/shift-issue-logs.api';
import {
  CreateShiftIssueLogRequestDto,
  ISSUE_TYPE_LABELS,
  IssueType
} from '../../../../core/dtos/shift-issue-logs/create-shift-issue-log-request.dto';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-log-issue-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './log-issue-form.component.html',
  styleUrl: './log-issue-form.component.css'
})
export class LogIssueFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly issueLogsApi = inject(ShiftIssueLogsApi);
  private readonly toast = inject(ToastService);

  @Input({ required: true }) shiftId!: number;
  @Output() submitted = new EventEmitter<void>();

  protected readonly submitting = signal(false);
  protected readonly issueTypeLabels = ISSUE_TYPE_LABELS;
  protected readonly issueTypeOptions: IssueType[] = [1, 2];

  protected readonly form = this.fb.nonNullable.group({
    issueType: [1 as IssueType, Validators.required],
    scrapQuantity: [0, [Validators.required, Validators.min(0)]],
    downtimeHours: [0, [Validators.min(0)]],
    downtimeMinutes: [0, [Validators.min(0), Validators.max(59)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]]
  }, { validators: [LogIssueFormComponent.scrapOrDowntimeRequired] });

  private static scrapOrDowntimeRequired(control: AbstractControl): ValidationErrors | null {
    const scrap = control.get('scrapQuantity')?.value ?? 0;
    const hours = control.get('downtimeHours')?.value ?? 0;
    const minutes = control.get('downtimeMinutes')?.value ?? 0;
    if (scrap <= 0 && hours <= 0 && minutes <= 0) {
      return { scrapOrDowntimeRequired: true };
    }
    return null;
  }

  protected onSubmit(): void {
    if (this.submitting()) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();

    const h = raw.downtimeHours;
    const m = raw.downtimeMinutes;
    const downtime = (h === 0 && m === 0)
      ? null
      : `${h.toString().padStart(2, '0')}:${m.toString().padStart(2, '0')}:00`;

    const dto: CreateShiftIssueLogRequestDto = {
      shiftId: this.shiftId,
      issueType: Number(raw.issueType) as IssueType,
      scrapQuantity: raw.scrapQuantity,
      description: raw.description,
      downtime
    };

    this.submitting.set(true);
    this.issueLogsApi.create(dto).subscribe({
      next: () => {
        this.toast.success('Issue logged');
        this.form.reset({
          issueType: 1,
          scrapQuantity: 0,
          downtimeHours: 0,
          downtimeMinutes: 0,
          description: ''
        });
        this.submitting.set(false);
        this.submitted.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err);
        this.submitting.set(false);
      }
    });
  }
}
