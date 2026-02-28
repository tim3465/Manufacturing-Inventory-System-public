import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ShippingReceivingApi } from '../../../../core/api/shipping-receiving.api';
import {
  ReceiveShipmentRequestDto,
  StockLotCondition,
  STOCK_LOT_CONDITIONS,
  STOCK_LOT_CONDITION_LABELS
} from '../../../../core/dtos/shipping-receiving';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-receive-shipment-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './receive-shipment-modal.component.html',
  styleUrl: './receive-shipment-modal.component.css'
})
export class ReceiveShipmentModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly shippingApi = inject(ShippingReceivingApi);
  private readonly toast = inject(ToastService);

  @Output() closed = new EventEmitter<void>();
  @Output() created = new EventEmitter<void>();

  protected readonly submitting = signal(false);

  protected readonly conditionOptions = STOCK_LOT_CONDITIONS;
  protected readonly conditionLabels = STOCK_LOT_CONDITION_LABELS;

  protected readonly form = this.fb.nonNullable.group({
    heatNumber: ['', [Validators.required, Validators.maxLength(100)]],
    materialName: ['', [Validators.required, Validators.maxLength(100)]],
    lotNumber: ['', [Validators.required, Validators.maxLength(100)]],
    amountOfBars: [1, [Validators.required, Validators.min(1)]],
    diameter: [null as number | null, [Validators.required]],
    barLength: [null as number | null, [Validators.required]],
    condition: [0 as StockLotCondition | 0, [Validators.required]],
    checkedInDateTime: ['', [Validators.required]],
    notes: ['']
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

    const v = this.form.getRawValue();

    const dto: ReceiveShipmentRequestDto = {
      materialId: null,
      heatNumber: v.heatNumber.trim(),
      materialName: v.materialName.trim(),
      lotNumber: v.lotNumber.trim(),
      amountOfBars: v.amountOfBars,
      diameter: v.diameter!,
      barLength: v.barLength!,
      condition: Number(v.condition) as StockLotCondition,
      checkedInDateTime: new Date(v.checkedInDateTime).toISOString(),
      notes: v.notes?.trim() || null
    };

    this.submitting.set(true);
    this.shippingApi.receive(dto).subscribe({
      next: () => {
        this.toast.success('Shipment received');
        this.created.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to receive shipment');
        this.submitting.set(false);
      }
    });
  }
}
