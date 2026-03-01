import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { StockLotAdjustmentsApi } from '../../../../core/api/stock-lot-adjustments.api';
import {
  AdjustmentReason,
  ADJUSTMENT_REASONS,
  ADJUSTMENT_REASON_LABELS,
  CreateStockLotAdjustmentRequestDto
} from '../../../../core/dtos/stock-lot-adjustments/create-stock-lot-adjustment-request.dto';
import { ToastService } from '../../../../core/ui/toast/toast.service';

type Direction = 'add' | 'remove';

@Component({
  selector: 'app-adjust-bars-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './adjust-bars-modal.component.html',
  styleUrl: './adjust-bars-modal.component.css'
})
export class AdjustBarsModalComponent {
  @Input({ required: true }) stockLotId!: number;
  @Input({ required: true }) lotNumber!: string;

  @Output() closed = new EventEmitter<void>();
  @Output() adjusted = new EventEmitter<void>();

  private readonly fb = inject(FormBuilder);
  private readonly adjustmentsApi = inject(StockLotAdjustmentsApi);
  private readonly toast = inject(ToastService);

  protected readonly submitting = signal(false);
  protected readonly direction = signal<Direction>('add');

  protected readonly reasonOptions = ADJUSTMENT_REASONS;
  protected readonly reasonLabels = ADJUSTMENT_REASON_LABELS;

  protected readonly form = this.fb.nonNullable.group({
    quantity: [1, [Validators.required, Validators.min(1)]],
    reason: [0 as AdjustmentReason | 0, [Validators.required]],
    notes: ['']
  });

  protected setDirection(dir: Direction): void {
    this.direction.set(dir);
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
    const deltaBars = this.direction() === 'add' ? v.quantity : -v.quantity;

    const dto: CreateStockLotAdjustmentRequestDto = {
      stockLotId: this.stockLotId,
      deltaBars,
      reason: Number(v.reason) as AdjustmentReason,
      notes: v.notes?.trim() || null
    };

    this.submitting.set(true);
    this.adjustmentsApi.create(dto).subscribe({
      next: () => {
        const action = this.direction() === 'add' ? 'added' : 'removed';
        this.toast.success(`${v.quantity} bar${v.quantity !== 1 ? 's' : ''} ${action}`);
        this.adjusted.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to adjust bars');
        this.submitting.set(false);
      }
    });
  }
}
