// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/StockLotsController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { StockLotDto } from '../dtos/stock-lots/stock-lot.dto';
import { StockLotSearchRequestDto } from '../dtos/stock-lots/stock-lot-search-request.dto';
import { StockLotSearchResultDto } from '../dtos/stock-lots/stock-lot-search-result.dto';
import { ApiClient } from './api-client.service';

export const STOCK_LOTS_PATH = '/StockLots';

@Injectable({ providedIn: 'root' })
export class StockLotsApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/StockLots - all active stock lots */
  listActive(): Observable<StockLotDto[]> {
    return this.api.getCached<StockLotDto[]>(STOCK_LOTS_PATH);
  }

  /** GET /api/StockLots/search - server-driven search with filtering, sorting, pagination */
  search(request: StockLotSearchRequestDto): Observable<StockLotSearchResultDto> {
    return this.api.get<StockLotSearchResultDto>(`${STOCK_LOTS_PATH}/search`, request as Record<string, string | number | boolean | null | undefined>);
  }
}
