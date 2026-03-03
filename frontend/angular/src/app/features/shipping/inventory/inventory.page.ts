import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { StockLotsApi } from '../../../core/api/stock-lots.api';
import { StockLotDto } from '../../../core/dtos/stock-lots/stock-lot.dto';
import { STOCK_LOT_CONDITION_LABELS, StockLotCondition } from '../../../core/dtos/shipping-receiving';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { ReceiveShipmentModalComponent } from './receive-shipment-modal/receive-shipment-modal.component';
import { AdjustBarsModalComponent } from './adjust-bars-modal/adjust-bars-modal.component';

@Component({
  selector: 'app-inventory-page',
  standalone: true,
  imports: [CommonModule, ReceiveShipmentModalComponent, AdjustBarsModalComponent],
  templateUrl: './inventory.page.html',
  styleUrl: './inventory.page.css'
})
export class InventoryPageComponent implements OnInit {
  private readonly stockLotsApi = inject(StockLotsApi);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal<boolean>(true);
  protected readonly error = signal<string | null>(null);
  protected readonly stockLots = signal<StockLotDto[]>([]);

  protected readonly isReceiveShipmentOpen = signal<boolean>(false);
  protected readonly isAdjustBarsOpen = signal<boolean>(false);
  protected readonly selectedLotForAdjustment = signal<StockLotDto | null>(null);

  protected readonly conditionLabels = STOCK_LOT_CONDITION_LABELS;

  ngOnInit(): void {
    this.loadInventory();
  }

  protected openReceiveShipment(): void {
    this.isReceiveShipmentOpen.set(true);
  }

  protected closeReceiveShipment(): void {
    this.isReceiveShipmentOpen.set(false);
  }

  protected openAdjustBars(lot: StockLotDto): void {
    this.selectedLotForAdjustment.set(lot);
    this.isAdjustBarsOpen.set(true);
  }

  protected closeAdjustBars(): void {
    this.isAdjustBarsOpen.set(false);
    this.selectedLotForAdjustment.set(null);
  }

  protected loadInventory(): void {
    this.loading.set(true);
    this.error.set(null);

    this.stockLotsApi.listActive().subscribe({
      next: (lots) => {
        this.stockLots.set(lots);
        this.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load inventory';
        this.error.set(message);
        this.toast.error(message);
        this.loading.set(false);
      }
    });
  }

  protected conditionLabel(condition: StockLotCondition): string {
    return this.conditionLabels[condition] ?? String(condition);
  }
}
