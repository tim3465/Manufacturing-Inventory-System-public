import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ShiftsApi } from '../../../core/api/shifts.api';
import { ToastService } from '../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-start-shift-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './start-shift-modal.component.html',
  styleUrl: './start-shift-modal.component.css'
})
export class StartShiftModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly shiftsApi = inject(ShiftsApi);
  private readonly toast = inject(ToastService);

  @Input({ required: true }) jobId!: number;
  @Input({ required: true }) machineSerialNumber!: string;
  @Input({ required: true }) partNumber!: string;
  @Output() closed = new EventEmitter<void>();
  @Output() started = new EventEmitter<void>();

  protected readonly submitting = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    startTime: ['', Validators.required]
  });

  ngOnInit(): void {
    this.setNow();
  }

  protected setNow(): void {
    const now = new Date();
    // Format as datetime-local string: YYYY-MM-DDTHH:mm
    const pad = (n: number) => n.toString().padStart(2, '0');
    const value = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}T${pad(now.getHours())}:${pad(now.getMinutes())}`;
    this.form.controls.startTime.setValue(value);
  }

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

    const { startTime } = this.form.getRawValue();

    this.submitting.set(true);
    this.shiftsApi.startShift({
      jobId: this.jobId,
      startTime: startTime + ':00'
    }).subscribe({
      next: () => {
        this.toast.success('Shift started');
        this.started.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to start shift');
        this.submitting.set(false);
      }
    });
  }
}
