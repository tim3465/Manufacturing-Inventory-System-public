import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { MachineDto, CreateMachineRequestDto } from '../dtos/machines/machine.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/machines';

@Injectable({ providedIn: 'root' })
export class MachinesApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/machines/all - all machines including inactive (Admin-only) */
  listAll(): Observable<MachineDto[]> {
    return this.api.getCached<MachineDto[]>(`${_PATH}/all`);
  }

  /** POST /api/machines - create machine (Admin-only) */
  create(dto: CreateMachineRequestDto): Observable<{ id: number }> {
    return this.api.post<{ id: number }>(`${_PATH}`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(`${_PATH}`);
        this.api.clearGetCache(`${_PATH}/all`);
      })
    );
  }

  /** PATCH /api/machines/{id}/inactivate - inactivate machine (Admin-only) */
  inactivate(id: number): Observable<void> {
    return this.api.patch<void>(`${_PATH}/${id}/inactivate`, {}).pipe(
      tap(() => {
        this.api.clearGetCache(`${_PATH}`);
        this.api.clearGetCache(`${_PATH}/all`);
      })
    );
  }

  /** PATCH /api/machines/{id}/activate - activate machine (Admin-only) */
  activate(id: number): Observable<void> {
    return this.api.patch<void>(`${_PATH}/${id}/activate`, {}).pipe(
      tap(() => {
        this.api.clearGetCache(`${_PATH}`);
        this.api.clearGetCache(`${_PATH}/all`);
      })
    );
  }
}
