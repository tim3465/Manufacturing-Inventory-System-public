import { SupervisorDashboardActiveJobDto } from './supervisor-dashboard-active-job.dto';

export interface SupervisorDashboardOperatorDto {
  operatorId: number;
  operatorName: string;
  machinesRunning: number;
  activeJobs: SupervisorDashboardActiveJobDto[];
  partsMadeToday: number;
  scrapToday: number;
  scrapPercentage: number;
}
