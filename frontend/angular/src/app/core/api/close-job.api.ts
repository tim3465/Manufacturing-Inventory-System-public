import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CloseJobRequestDto } from '../dtos/close-job/close-job-request.dto';
import { CloseJobResponseDto } from '../dtos/close-job/close-job-response.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/closejob';
const _SHIFTS_PATH = '/shifts';
const _MACHINES_WITH_JOBS_PATH = '/machines/with-jobs';
const _SUPERVISOR_DASHBOARD_PATH = '/supervisordashboard';
const _JOBS_MY_JOBS_PATH = '/jobs/my-jobs';

@Injectable({ providedIn: 'root' })
export class CloseJobApi {
  constructor(private readonly api: ApiClient) {}

  /** POST /api/closejob/close - close a job and its current shift (Machinist, Admin) */
  closeJob(dto: CloseJobRequestDto): Observable<CloseJobResponseDto> {
    return this.api.post<CloseJobResponseDto>(`${_PATH}/close`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(`${_SHIFTS_PATH}/running`);
        this.api.clearGetCache(`${_SHIFTS_PATH}/${dto.shiftId}/running`);
        this.api.clearGetCache(`${_SHIFTS_PATH}/my-logs`);
        this.api.clearGetCache(_MACHINES_WITH_JOBS_PATH);
        this.api.clearGetCache(_SUPERVISOR_DASHBOARD_PATH);
        this.api.clearGetCache(_JOBS_MY_JOBS_PATH);
      })
    );
  }
}
