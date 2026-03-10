// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/JobsController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { JobProductionDto } from '../dtos/jobs/job-production.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/jobs';

@Injectable({ providedIn: 'root' })
export class JobsApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/jobs/production - active jobs with shifts (Supervisor/Admin) */
  listProduction(): Observable<JobProductionDto[]> {
    return this.api.get<JobProductionDto[]>(`${_PATH}/production`);
  }
}
