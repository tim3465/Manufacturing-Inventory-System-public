import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateOrderWithJobsRequestDto, CreateOrderWithJobsResponseDto } from '../dtos/order-planning/order-planning.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/orderplanning';

@Injectable({ providedIn: 'root' })
export class OrderPlanningApi {
  constructor(private readonly api: ApiClient) {}

  /** POST /api/orderplanning/create - create order with jobs atomically (Supervisor/Admin) */
  createOrderWithJobs(dto: CreateOrderWithJobsRequestDto): Observable<CreateOrderWithJobsResponseDto> {
    return this.api.post<CreateOrderWithJobsResponseDto>(`${_PATH}/create`, dto);
  }
}
