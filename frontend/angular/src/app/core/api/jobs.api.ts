// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/JobsController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { JobProductionDto } from '../dtos/jobs/job-production.dto';
import { JobProductionSearchRequestDto } from '../dtos/jobs/job-production-search-request.dto';
import { JobProductionSearchResultDto } from '../dtos/jobs/job-production-search-result.dto';
import { JobShiftDto } from '../dtos/jobs/job-shift.dto';
import { JobReportDto } from '../dtos/jobs/job-report.dto';
import { MyJobListItemDto } from '../dtos/jobs/my-job.dto';
import { MyJobSearchRequestDto } from '../dtos/jobs/my-job-search-request.dto';
import { MyJobSearchResultDto } from '../dtos/jobs/my-job-search-result.dto';
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

  /** GET /api/jobs/production/search - paginated/filtered/sorted production jobs (Supervisor/Admin) */
  searchProduction(request: JobProductionSearchRequestDto): Observable<JobProductionSearchResultDto> {
    return this.api.get<JobProductionSearchResultDto>(`${_PATH}/production/search`, request as unknown as Record<string, string | number | boolean | null | undefined>);
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

  /** GET /api/jobs/{id}/report - full job report with totals and shift history (Supervisor/Admin) */
  getReport(id: number): Observable<JobReportDto> {
    return this.api.getCached<JobReportDto>(`${_PATH}/${id}/report`);
  }

  /** GET /api/jobs/my-jobs/search - paginated/filtered/sorted my jobs (Machinist/Admin) */
  searchMyJobs(request: MyJobSearchRequestDto): Observable<MyJobSearchResultDto> {
    return this.api.get<MyJobSearchResultDto>(`${_PATH}/my-jobs/search`, request as unknown as Record<string, string | number | boolean | null | undefined>);
  }
}
