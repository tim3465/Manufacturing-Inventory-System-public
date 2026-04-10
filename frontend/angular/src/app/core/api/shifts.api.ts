import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { RunningShiftDto } from '../dtos/shifts/running-shift.dto';
import { ShiftLogDto } from '../dtos/shifts/shift-log.dto';
import { ShiftLogSearchRequestDto } from '../dtos/shifts/shift-log-search-request.dto';
import { ShiftLogSearchResultDto } from '../dtos/shifts/shift-log-search-result.dto';
import { ShiftProductionSearchRequestDto } from '../dtos/shifts/shift-production-search-request.dto';
import { ShiftProductionSearchResultDto } from '../dtos/shifts/shift-production-search-result.dto';
import { ShiftDto } from '../dtos/shifts/shift.dto';
import { StartShiftRequestDto } from '../dtos/shifts/start-shift-request.dto';
import { UpdateShiftRequestDto } from '../dtos/shifts/update-shift-request.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/shifts';
const _MACHINES_WITH_JOBS_PATH = '/machines/with-jobs';
const _SUPERVISOR_DASHBOARD_PATH = '/supervisordashboard';

@Injectable({ providedIn: 'root' })
export class ShiftsApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/shifts/production - shifts with operator/job/part info (Supervisor/Admin) */
  listProduction(): Observable<ShiftDto[]> {
    return this.api.getCached<ShiftDto[]>(`${_PATH}/production`);
  }

  /** GET /api/shifts/production/search - paginated/filtered/sorted production shifts (Supervisor/Admin) */
  searchProduction(request: ShiftProductionSearchRequestDto): Observable<ShiftProductionSearchResultDto> {
    return this.api.get<ShiftProductionSearchResultDto>(
      `${_PATH}/production/search`,
      request as unknown as Record<string, string | number | boolean | null | undefined>
    );
  }

  /** POST /api/shifts/start - start a new shift (Machinist, Admin) */
  startShift(dto: StartShiftRequestDto): Observable<{ id: number }> {
    return this.api.post<{ id: number }>(`${_PATH}/start`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(`${_PATH}/running`);
        this.api.clearGetCache(_MACHINES_WITH_JOBS_PATH);
        this.api.clearGetCache(_SUPERVISOR_DASHBOARD_PATH);
      })
    );
  }

  /** GET /api/shifts/running - all running shifts (Machinist, Admin) */
  listRunning(): Observable<RunningShiftDto[]> {
    return this.api.getCached<RunningShiftDto[]>(`${_PATH}/running`);
  }

  /** GET /api/shifts/{id}/running - single running shift (Machinist, Admin) */
  getRunning(id: number): Observable<RunningShiftDto> {
    return this.api.getCached<RunningShiftDto>(`${_PATH}/${id}/running`);
  }

  /** PATCH /api/shifts/{id}/save - save shift progress (Machinist, Admin) */
  saveShift(id: number, dto: UpdateShiftRequestDto): Observable<void> {
    return this.api.patch<void>(`${_PATH}/${id}/save`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(`${_PATH}/running`);
        this.api.clearGetCache(`${_PATH}/${id}/running`);
      })
    );
  }

  /** PATCH /api/shifts/{id}/close - close a shift (Machinist, Admin) */
  closeShift(id: number, dto: UpdateShiftRequestDto): Observable<void> {
    return this.api.patch<void>(`${_PATH}/${id}/close`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(`${_PATH}/running`);
        this.api.clearGetCache(`${_PATH}/${id}/running`);
        this.api.clearGetCache(`${_PATH}/my-logs`);
        this.api.clearGetCache(_MACHINES_WITH_JOBS_PATH);
        this.api.clearGetCache(_SUPERVISOR_DASHBOARD_PATH);
      })
    );
  }

  /** GET /api/shifts/my-logs - current user's shift log (Machinist, Admin) */
  listMyLogs(): Observable<ShiftLogDto[]> {
    return this.api.getCached<ShiftLogDto[]>(`${_PATH}/my-logs`);
  }

  /** GET /api/shifts/my-logs/search - paginated/filtered shift log search (Machinist, Admin) */
  searchMyLogs(request: ShiftLogSearchRequestDto): Observable<ShiftLogSearchResultDto> {
    return this.api.get<ShiftLogSearchResultDto>(
      `${_PATH}/my-logs/search`,
      request as unknown as Record<string, string | number | boolean | null | undefined>
    );
  }
}
