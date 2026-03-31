// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/OrdersController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OrderProductionDto } from '../dtos/orders/order-production.dto';
import { OrderProductionSearchRequestDto } from '../dtos/orders/order-production-search-request.dto';
import { OrderProductionSearchResultDto } from '../dtos/orders/order-production-search-result.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/orders';

@Injectable({ providedIn: 'root' })
export class OrdersApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/orders/production - active orders with production metrics (Supervisor/Admin) */
  listProduction(): Observable<OrderProductionDto[]> {
    return this.api.get<OrderProductionDto[]>(`${_PATH}/production`);
  }

  /** GET /api/orders/production/search - paginated, filtered, sorted orders (Supervisor/Admin) */
  searchProduction(request: OrderProductionSearchRequestDto): Observable<OrderProductionSearchResultDto> {
    return this.api.get<OrderProductionSearchResultDto>(
      `${_PATH}/production/search`,
      request as unknown as Record<string, string | number | boolean | null | undefined>
    );
  }
}
