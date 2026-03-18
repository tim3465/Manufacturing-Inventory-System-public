import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PartsApi } from '../../../../core/api/parts.api';
import { CreatePartRequestDto } from '../../../../core/dtos/parts/create-part-request.dto';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-add-part-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-part-modal.component.html',
  styleUrl: './add-part-modal.component.css'
})
export class AddPartModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly partsApi = inject(PartsApi);
  private readonly toast = inject(ToastService);

  @Output() closed = new EventEmitter<void>();
  @Output() created = new EventEmitter<void>();

  protected readonly submitting = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    partName: ['', [Validators.required, Validators.maxLength(100)]],
    partNumber: ['', [Validators.required, Validators.maxLength(50)]],
    approxPartCycleTime: ['00:00:30', [Validators.required, Validators.pattern(/^\d{2}:\d{2}:\d{2}$/)]],
    checkPerPart: [1, [Validators.required, Validators.min(0)]]
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

    const value = this.form.getRawValue();
    const dto: CreatePartRequestDto = {
      partName: value.partName.trim(),
      partNumber: value.partNumber.trim(),
      approxPartCycleTime: value.approxPartCycleTime.trim(),
      checkPerPart: Number(value.checkPerPart)
    };

    this.submitting.set(true);
    this.partsApi.create(dto).subscribe({
      next: () => {
        this.toast.success('Part created');
        this.created.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to create part');
        this.submitting.set(false);
      }
    });
  }
}
