import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShiftDto } from '../dtos/shifts/shift.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/shifts';

@Injectable({ providedIn: 'root' })
export class ShiftsApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/shifts/production - shifts with operator/job/part info (Supervisor/Admin) */
  listProduction(): Observable<ShiftDto[]> {
    return this.api.getCached<ShiftDto[]>(`${_PATH}/production`);
  }
}
