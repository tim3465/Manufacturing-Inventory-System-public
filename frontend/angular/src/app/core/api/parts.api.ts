// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/PartsController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
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
}
