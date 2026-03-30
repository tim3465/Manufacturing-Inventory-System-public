import { SupervisorDashboardOperatorDto } from './supervisor-dashboard-operator.dto';
import { SupervisorDashboardOrderDto } from './supervisor-dashboard-order.dto';

export interface SupervisorDashboardDto {
  machinesRunning: number;
  operatorsActive: number;
  lateJobs: number;
  operators: SupervisorDashboardOperatorDto[];
  orders: SupervisorDashboardOrderDto[];
}
