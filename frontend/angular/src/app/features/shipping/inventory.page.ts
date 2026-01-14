import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-inventory-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './inventory.page.html',
  styleUrl: './inventory.page.css'
})
export class InventoryPageComponent {
  protected readonly items = [
    { sku: 'AL-6061', description: 'Aluminum Bar 1"x12"', qty: 24 },
    { sku: 'ST-304', description: 'Stainless Plate 8"x8"', qty: 10 },
    { sku: 'BR-260', description: 'Brass Rod 0.5"x36"', qty: 18 }
  ];
}

