import { SupervisorDashboardOperatorDto } from './supervisor-dashboard-operator.dto';

export interface SupervisorDashboardDto {
  machinesRunning: number;
  operatorsActive: number;
  lateJobs: number;
  operators: SupervisorDashboardOperatorDto[];
}
