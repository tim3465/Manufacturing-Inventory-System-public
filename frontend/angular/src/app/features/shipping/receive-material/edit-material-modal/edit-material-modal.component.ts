import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MaterialsApi } from '../../../../core/api/materials.api';
import { UpdateMaterialRequestDto } from '../../../../core/dtos/materials';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-edit-material-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-material-modal.component.html',
  styleUrl: './edit-material-modal.component.css'
})
export class EditMaterialModalComponent implements OnInit {
  @Input({ required: true }) materialId!: number;
  @Input({ required: true }) heatNumber!: string;
  @Input({ required: true }) materialName!: string;

  @Output() closed = new EventEmitter<void>();
  @Output() updated = new EventEmitter<void>();

  private readonly fb = inject(FormBuilder);
  private readonly materialsApi = inject(MaterialsApi);
  private readonly toast = inject(ToastService);

  protected readonly submitting = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    heatNumber: ['', [Validators.required, Validators.maxLength(100)]],
    materialName: ['', [Validators.required, Validators.maxLength(100)]]
  });

  ngOnInit(): void {
    this.form.setValue({ heatNumber: this.heatNumber, materialName: this.materialName });
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

    const v = this.form.getRawValue();
    const dto: UpdateMaterialRequestDto = {
      heatNumber: v.heatNumber.trim(),
      materialName: v.materialName.trim()
    };

    this.submitting.set(true);
    this.materialsApi.update(this.materialId, dto).subscribe({
      next: () => {
        this.toast.success('Material updated');
        this.updated.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to update material');
        this.submitting.set(false);
      }
    });
  }
}
