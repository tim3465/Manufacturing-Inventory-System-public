import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { CloseJobApi } from '../../../../core/api/close-job.api';
import { CloseJobRequestDto } from '../../../../core/dtos/close-job/close-job-request.dto';
import { UpdateShiftRequestDto } from '../../../../core/dtos/shifts/update-shift-request.dto';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-close-job-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './close-job-modal.component.html',
  styleUrl: './close-job-modal.component.css'
})
export class CloseJobModalComponent {
  private readonly closeJobApi = inject(CloseJobApi);
  private readonly toast = inject(ToastService);

  @Input({ required: true }) shiftId!: number;
  @Input({ required: true }) jobId!: number;
  @Input({ required: true }) shiftData!: UpdateShiftRequestDto;
  @Input({ required: true }) machineName!: string;
  @Input({ required: true }) partName!: string;

  @Output() closed = new EventEmitter<void>();
  @Output() jobClosed = new EventEmitter<void>();

  protected readonly submitting = signal<boolean>(false);

  protected onCancel(): void {
    if (this.submitting()) return;
    this.closed.emit();
  }

  protected onConfirm(): void {
    if (this.submitting()) return;

    this.submitting.set(true);

    const dto: CloseJobRequestDto = {
      shiftId: this.shiftId,
      jobId: this.jobId,
      shiftData: this.shiftData
    };

    this.closeJobApi.closeJob(dto).subscribe({
      next: () => {
        this.toast.success('Job closed');
        this.jobClosed.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to close job');
        this.submitting.set(false);
      }
    });
  }
}
