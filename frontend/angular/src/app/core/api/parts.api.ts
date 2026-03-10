import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CreatePartRequestDto } from '../dtos/parts/create-part-request.dto';
import { PartDto } from '../dtos/parts/part.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/parts';

@Injectable({ providedIn: 'root' })
export class PartsApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/parts - active parts */
  listActive(): Observable<PartDto[]> {
    return this.api.getCached<PartDto[]>(_PATH);
  }

  /** POST /api/parts - create part (Admin/Supervisor) */
  create(dto: CreatePartRequestDto): Observable<PartDto> {
    return this.api.post<PartDto>(_PATH, dto).pipe(
      tap(() => {
        this.api.clearGetCache(_PATH);
        this.api.clearGetCache(`${_PATH}/all`);
      })
    );
  }
}
