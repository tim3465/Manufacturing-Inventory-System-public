import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CustomerDto, CreateCustomerRequestDto, UpdateCustomerRequestDto } from '../dtos/customers/customer.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/customers';

@Injectable({ providedIn: 'root' })
export class CustomersApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/customers - active customers */
  listActive(): Observable<CustomerDto[]> {
    return this.api.getCached<CustomerDto[]>(_PATH);
  }

  /** GET /api/customers/{id} - get by id */
  getById(id: number): Observable<CustomerDto> {
    return this.api.get<CustomerDto>(`${_PATH}/${id}`);
  }

  /** POST /api/customers - create customer (Supervisor/Admin) */
  create(dto: CreateCustomerRequestDto): Observable<number> {
    return this.api.post<number>(_PATH, dto).pipe(
      tap(() => {
        this.api.clearGetCache(_PATH);
      })
    );
  }

  /** PATCH /api/customers/{id} - update customer (Supervisor/Admin) */
  update(id: number, dto: UpdateCustomerRequestDto): Observable<CustomerDto> {
    return this.api.patch<CustomerDto>(`${_PATH}/${id}`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(_PATH);
      })
    );
  }
}
