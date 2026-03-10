// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/OrdersController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderProductionDto } from '../dtos/orders/order-production.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/orders';

@Injectable({ providedIn: 'root' })
export class OrdersApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/orders/production - active orders with production metrics (Supervisor/Admin) */
  listProduction(): Observable<OrderProductionDto[]> {
    return this.api.get<OrderProductionDto[]>(`${_PATH}/production`);
  }
}
