// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/JobsController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { JobProductionDto } from '../dtos/jobs/job-production.dto';
import { JobShiftDto } from '../dtos/jobs/job-shift.dto';
import { MyJobListItemDto } from '../dtos/jobs/my-job.dto';
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
  /** PATCH /api/jobs/{id}/assign-stocklot - assign or clear a stock lot on a job (Supervisor/Admin) */
  assignStockLot(id: number, body: { stockLotId: number | null }): Observable<void> {
    return this.api.patch<void>(`${_PATH}/${id}/assign-stocklot`, body);

  }

  /** GET /api/jobs/by-order/{orderId} - jobs for a specific order (Supervisor/Admin) */
  listByOrder(orderId: number): Observable<JobProductionDto[]> {
    return this.api.get<JobProductionDto[]>(`${_PATH}/by-order/${orderId}`);
  }

  /** GET /api/jobs/my-jobs - jobs assigned to the authenticated machinist (Machinist/Admin) */
  listMyJobs(): Observable<MyJobListItemDto[]> {
    return this.api.getCached<MyJobListItemDto[]>(`${_PATH}/my-jobs`);
  }

  /** GET /api/jobs/my-jobs/{jobId}/shifts - shifts for a specific job (Machinist/Admin) */
  getMyJobShifts(jobId: number): Observable<JobShiftDto[]> {
    return this.api.get<JobShiftDto[]>(`${_PATH}/my-jobs/${jobId}/shifts`);
  }
}
