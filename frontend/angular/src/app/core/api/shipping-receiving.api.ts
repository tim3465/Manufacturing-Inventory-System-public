import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import {
  ReceiveShipmentRequestDto,
  ReceiveShipmentResponseDto
} from '../dtos/shipping-receiving';
import { ApiClient } from './api-client.service';
import { STOCK_LOTS_PATH } from './stock-lots.api';

const _PATH = '/ShippingReceiving';

@Injectable({ providedIn: 'root' })
export class ShippingReceivingApi {
  constructor(private readonly api: ApiClient) {}

  /** POST /api/ShippingReceiving/receive - receive a shipment (Admin or Shipping role) */
  receive(dto: ReceiveShipmentRequestDto): Observable<ReceiveShipmentResponseDto> {
    return this.api.post<ReceiveShipmentResponseDto>(`${_PATH}/receive`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(STOCK_LOTS_PATH);
      })
    );
  }
}
