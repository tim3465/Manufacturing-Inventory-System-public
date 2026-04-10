import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MachinesApi } from '../../../../core/api/machines.api';
import { CreateMachineRequestDto } from '../../../../core/dtos/machines/machine.dto';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-add-machine-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-machine-modal.component.html',
  styleUrl: './add-machine-modal.component.css'
})
export class AddMachineModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly machinesApi = inject(MachinesApi);
  private readonly toast = inject(ToastService);

  @Output() closed = new EventEmitter<void>();
  @Output() created = new EventEmitter<void>();

  protected readonly submitting = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    serialNumber: ['', [Validators.required, Validators.maxLength(100)]],
    modelNumber: ['', [Validators.required, Validators.maxLength(100)]]
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
    const dto: CreateMachineRequestDto = {
      serialNumber: value.serialNumber.trim(),
      modelNumber: value.modelNumber.trim()
    };

    this.submitting.set(true);
    this.machinesApi.create(dto).subscribe({
      next: () => {
        this.toast.success('Machine created');
        this.created.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to create machine');
        this.submitting.set(false);
      }
    });
  }
}
