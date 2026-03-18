// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/JobsController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { JobProductionDto } from '../dtos/jobs/job-production.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/jobs';
const _MACHINES_WITH_JOBS_PATH = '/machines/with-jobs';

export interface StartJobResponseDto {
  jobId: number;
  stockLotAdjustmentId: number;
}

@Injectable({ providedIn: 'root' })
export class JobsApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/jobs/production - active jobs with shifts (Supervisor/Admin) */
  listProduction(): Observable<JobProductionDto[]> {
    return this.api.get<JobProductionDto[]>(`${_PATH}/production`);
  }

  /** POST /api/jobs/{id}/start - activate a job and pull bars from inventory */
  startJob(jobId: number, barsToAdd: number): Observable<StartJobResponseDto> {
    return this.api.post<StartJobResponseDto>(`${_PATH}/${jobId}/start`, { barsToAdd }).pipe(
      tap(() => {
        this.api.clearGetCache(_MACHINES_WITH_JOBS_PATH);
      })
    );
  }
}
