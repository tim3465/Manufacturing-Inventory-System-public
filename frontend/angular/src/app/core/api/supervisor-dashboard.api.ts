import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SupervisorDashboardDto } from '../dtos/supervisor-dashboard/supervisor-dashboard.dto';
import { ApiClient } from './api-client.service';

const _PATH = '/supervisordashboard';

@Injectable({ providedIn: 'root' })
export class SupervisorDashboardApi {
  constructor(private readonly api: ApiClient) {}

  getDashboard(): Observable<SupervisorDashboardDto> {
    return this.api.getCached<SupervisorDashboardDto>(_PATH);
  }
}
