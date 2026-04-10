import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CreateShiftIssueLogRequestDto } from '../dtos/shift-issue-logs/create-shift-issue-log-request.dto';
import { ShiftIssueLogDto } from '../dtos/shift-issue-logs/shift-issue-log.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/ShiftIssueLogs';
const _BY_SHIFT_PATH = (id: number) => `/ShiftIssueLogs/by-shift/${id}`;
const _SHIFTS_RUNNING_PATH = '/shifts/running';

@Injectable({ providedIn: 'root' })
export class ShiftIssueLogsApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/ShiftIssueLogs/by-shift/{shiftId} - list issue logs for a shift (Machinist, Admin) */
  getByShift(shiftId: number): Observable<ShiftIssueLogDto[]> {
    return this.api.getCached<ShiftIssueLogDto[]>(_BY_SHIFT_PATH(shiftId));
  }

  /** POST /api/ShiftIssueLogs - log a scrap/downtime issue (Machinist, Admin) */
  create(dto: CreateShiftIssueLogRequestDto): Observable<{ id: number }> {
    return this.api.post<{ id: number }>(_PATH, dto).pipe(
      tap(() => {
        this.api.clearGetCache(_SHIFTS_RUNNING_PATH);
        this.api.clearGetCache(`/shifts/${dto.shiftId}/running`);
        this.api.clearGetCache(_BY_SHIFT_PATH(dto.shiftId));
      })
    );
  }
}
