import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { SupervisorDashboardApi } from '../../core/api/supervisor-dashboard.api';
import { SupervisorDashboardActiveJobDto } from '../../core/dtos/supervisor-dashboard/supervisor-dashboard-active-job.dto';
import { SupervisorDashboardDto } from '../../core/dtos/supervisor-dashboard/supervisor-dashboard.dto';
import { ToastService } from '../../core/ui/toast/toast.service';
import { DonutRingComponent } from '../../core/ui/donut-ring/donut-ring.component';

interface OperatorCard {
  operatorId: number;
  operatorName: string;
  machinesRunning: number;
  activeJobs: SupervisorDashboardActiveJobDto[];
  partsMadeToday: number;
  scrapToday: number;
  scrapPercentage: string;
}

interface OrderCard {
  orderId: number;
  partName: string;
  customerName: string;
  target: number;
  goodParts: number;
  scrap: number;
}

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, DonutRingComponent],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.css'
})
export class DashboardPageComponent implements OnInit {
  private readonly supervisorDashboardApi = inject(SupervisorDashboardApi);
  private readonly toast = inject(ToastService);
  private readonly auth = inject(AuthService);

  protected readonly currentUserId = computed(() => this.auth.getUserId());

  protected readonly loading = signal(true);
  protected readonly dashboard = signal<SupervisorDashboardDto | null>(null);

  protected readonly orderCards = computed<OrderCard[]>(() => {
    const data = this.dashboard();
    if (!data?.orders) return [];
    return data.orders.map((o) => ({
      orderId: o.orderId,
      partName: o.partName,
      customerName: o.customerName,
      target: o.target,
      goodParts: o.goodParts,
      scrap: o.scrap
    }));
  });

  protected readonly operatorCards = computed<OperatorCard[]>(() => {
    const data = this.dashboard();
    if (!data) return [];
    return data.operators.map((op) => ({
      operatorId: op.operatorId,
      operatorName: op.operatorName,
      machinesRunning: op.machinesRunning,
      activeJobs: op.activeJobs,
      partsMadeToday: op.partsMadeToday,
      scrapToday: op.scrapToday,
      scrapPercentage: `${op.scrapPercentage.toFixed(1)}%`
    }));
  });

  ngOnInit(): void {
    this.supervisorDashboardApi.getDashboard().subscribe({
      next: (data) => {
        this.dashboard.set(data);
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err);
        this.loading.set(false);
      }
    });
  }
}
