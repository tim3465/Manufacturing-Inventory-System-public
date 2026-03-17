import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { JobsApi } from '../../../../core/api/jobs.api';
import { StockLotsApi } from '../../../../core/api/stock-lots.api';
import { StockLotDto } from '../../../../core/dtos/stock-lots/stock-lot.dto';
import { ToastService } from '../../../../core/ui/toast/toast.service';
import { JobProductionRow } from '../production.page';

@Component({
  selector: 'app-assign-stock-lot-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './assign-stock-lot-modal.component.html',
  styleUrl: './assign-stock-lot-modal.component.css'
})
export class AssignStockLotModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly jobsApi = inject(JobsApi);
  private readonly stockLotsApi = inject(StockLotsApi);
  private readonly toast = inject(ToastService);

  @Input({ required: true }) job!: JobProductionRow;
  @Output() closed = new EventEmitter<void>();
  @Output() updated = new EventEmitter<void>();

  protected readonly submitting = signal(false);
  protected readonly stockLots = signal<StockLotDto[]>([]);

  protected readonly form = this.fb.group({
    stockLotId: [null as number | null]
  });

  ngOnInit(): void {
    this.stockLotsApi.listActive().subscribe(lots => {
      this.stockLots.set(lots);
      this.form.patchValue({ stockLotId: this.job.stockLotId });
    });
  }

  protected onCancel(): void {
    if (this.submitting()) return;
    this.closed.emit();
  }

  protected submit(): void {
    if (this.submitting()) return;
    this.submitting.set(true);
    const { stockLotId } = this.form.getRawValue();
    this.jobsApi.assignStockLot(this.job.id, { stockLotId }).subscribe({
      next: () => {
        this.toast.success('Stock lot assigned');
        this.updated.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to assign stock lot');
        this.submitting.set(false);
      }
    });
  }
}
