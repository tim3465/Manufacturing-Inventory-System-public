import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { JobsApi } from '../../../core/api/jobs.api';
import { ToastService } from '../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-start-job-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './start-job-modal.component.html',
  styleUrl: './start-job-modal.component.css'
})
export class StartJobModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly jobsApi = inject(JobsApi);
  private readonly toast = inject(ToastService);

  @Input({ required: true }) jobId!: number;
  @Input({ required: true }) barAmountPlanned!: number;
  @Output() closed = new EventEmitter<void>();
  @Output() started = new EventEmitter<void>();

  protected readonly submitting = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    barsToAdd: [null as unknown as number, [Validators.required, Validators.min(1)]]
  });

  protected onCancel(): void {
    if (this.submitting()) return;
    this.closed.emit();
  }

  protected onSubmit(): void {
    if (this.submitting()) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { barsToAdd } = this.form.getRawValue();

    this.submitting.set(true);
    this.jobsApi.startJob(this.jobId, barsToAdd).subscribe({
      next: () => {
        this.toast.success('Job started');
        this.started.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to start job');
        this.submitting.set(false);
      }
    });
  }
}
