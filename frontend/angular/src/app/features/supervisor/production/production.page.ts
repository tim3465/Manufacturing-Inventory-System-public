import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { OrdersApi } from '../../../core/api/orders.api';
import { OrderProductionDto } from '../../../core/dtos/orders/order-production.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-production-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './production.page.html',
  styleUrl: './production.page.css'
})
export class ProductionPageComponent implements OnInit {
  private readonly ordersApi = inject(OrdersApi);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal<boolean>(true);
  protected readonly error = signal<string | null>(null);
  protected readonly orders = signal<OrderProductionDto[]>([]);

  ngOnInit(): void {
    this.loadOrders();
  }

  protected loadOrders(): void {
    this.loading.set(true);
    this.error.set(null);

    this.ordersApi.listProduction().subscribe({
      next: (orders) => {
        this.orders.set(orders);
        this.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load production data';
        this.error.set(message);
        this.toast.error(message);
        this.loading.set(false);
      }
    });
  }

  protected clampPercent(value: number): number {
    return Math.min(100, Math.max(0, value));
  }
}
