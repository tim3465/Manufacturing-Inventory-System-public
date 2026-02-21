import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-orders-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orders.page.html',
  styleUrl: './orders.page.css'
})
export class OrdersPageComponent {
  protected readonly orders = [
    { id: 'SO-2104', customer: 'Acme Co', status: 'Planning' },
    { id: 'SO-2105', customer: 'Helix Parts', status: 'Released' },
    { id: 'SO-2106', customer: 'Nova Flight', status: 'Review' }
  ];
}

