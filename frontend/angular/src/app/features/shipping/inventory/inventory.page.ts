import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { ReceiveShipmentModalComponent } from './receive-shipment-modal/receive-shipment-modal.component';

@Component({
  selector: 'app-inventory-page',
  standalone: true,
  imports: [CommonModule, ReceiveShipmentModalComponent],
  templateUrl: './inventory.page.html',
  styleUrl: './inventory.page.css'
})
export class InventoryPageComponent {
  protected readonly items = [
    { sku: 'AL-6061', description: 'Aluminum Bar 1"x12"', qty: 24 },
    { sku: 'ST-304', description: 'Stainless Plate 8"x8"', qty: 10 },
    { sku: 'BR-260', description: 'Brass Rod 0.5"x36"', qty: 18 }
  ];

  protected readonly isReceiveShipmentOpen = signal<boolean>(false);

  protected openReceiveShipment(): void {
    this.isReceiveShipmentOpen.set(true);
  }

  protected closeReceiveShipment(): void {
    this.isReceiveShipmentOpen.set(false);
  }

  protected loadInventory(): void {
    // TODO: reload inventory data from API once stock-lots endpoint is wired up
  }
}
