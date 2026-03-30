import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CreateShiftIssueLogRequestDto } from '../dtos/shift-issue-logs/create-shift-issue-log-request.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/ShiftIssueLogs';
const _SHIFTS_RUNNING_PATH = '/shifts/running';

@Injectable({ providedIn: 'root' })
export class ShiftIssueLogsApi {
  constructor(private readonly api: ApiClient) {}

  /** POST /api/ShiftIssueLogs - log a scrap/downtime issue (Machinist, Admin) */
  create(dto: CreateShiftIssueLogRequestDto): Observable<{ id: number }> {
    return this.api.post<{ id: number }>(_PATH, dto).pipe(
      tap(() => {
        this.api.clearGetCache(_SHIFTS_RUNNING_PATH);
        this.api.clearGetCache(`/shifts/${dto.shiftId}/running`);
      })
    );
  }
}
