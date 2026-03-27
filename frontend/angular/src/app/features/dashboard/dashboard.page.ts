import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { SupervisorDashboardApi } from '../../core/api/supervisor-dashboard.api';
import { SupervisorDashboardActiveJobDto } from '../../core/dtos/supervisor-dashboard/supervisor-dashboard-active-job.dto';
import { SupervisorDashboardDto } from '../../core/dtos/supervisor-dashboard/supervisor-dashboard.dto';
import { ToastService } from '../../core/ui/toast/toast.service';

interface OperatorCard {
  operatorId: number;
  operatorName: string;
  machinesRunning: number;
  activeJobs: SupervisorDashboardActiveJobDto[];
  partsMadeToday: number;
  scrapToday: number;
  scrapPercentage: string;
}

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.css'
})
export class DashboardPageComponent implements OnInit {
  private readonly supervisorDashboardApi = inject(SupervisorDashboardApi);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal(true);
  protected readonly dashboard = signal<SupervisorDashboardDto | null>(null);

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
