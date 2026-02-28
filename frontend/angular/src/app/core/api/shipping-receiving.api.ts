import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ReceiveShipmentRequestDto,
  ReceiveShipmentResponseDto
} from '../dtos/shipping-receiving';
import { ApiClient } from './api-client.service';

const _PATH = '/ShippingReceiving';

@Injectable({ providedIn: 'root' })
export class ShippingReceivingApi {
  constructor(private readonly api: ApiClient) {}

  /** POST /api/ShippingReceiving/receive - receive a shipment (Admin-only) */
  receive(dto: ReceiveShipmentRequestDto): Observable<ReceiveShipmentResponseDto> {
    return this.api.post<ReceiveShipmentResponseDto>(`${_PATH}/receive`, dto);
  }
}
