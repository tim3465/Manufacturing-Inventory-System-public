// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/StockLotAdjustmentsController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CreateStockLotAdjustmentRequestDto } from '../dtos/stock-lot-adjustments/create-stock-lot-adjustment-request.dto';
import { ApiClient } from './api-client.service';
import { STOCK_LOTS_PATH } from './stock-lots.api';

const _PATH = '/StockLotAdjustments';

@Injectable({ providedIn: 'root' })
export class StockLotAdjustmentsApi {
  constructor(private readonly api: ApiClient) {}

  /** POST /api/StockLotAdjustments - create an adjustment (Admin or Shipping role) */
  create(dto: CreateStockLotAdjustmentRequestDto): Observable<{ id: number }> {
    return this.api.post<{ id: number }>(_PATH, dto).pipe(
      tap(() => {
        this.api.clearGetCache(STOCK_LOTS_PATH);
      })
    );
  }
}
