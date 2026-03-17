import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MachinesApi } from '../../core/api/machines.api';
import { MachineWithJobsDto } from '../../core/dtos/machines/machine-with-jobs.dto';
import { ToastService } from '../../core/ui/toast/toast.service';

interface JobRow {
  id: number;
  partNumber: string;
  dueDate: string;
  lotNumber: string | null;
}

interface MachineCard {
  id: number;
  name: string;
  modelNumber: string;
  jobs: JobRow[];
}

@Component({
  selector: 'app-machinist-machines-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './machines.page.html',
  styleUrl: './machines.page.css'
})
export class MachinistMachinesPageComponent implements OnInit {
  private readonly machinesApi = inject(MachinesApi);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal<boolean>(true);
  protected readonly machines = signal<MachineWithJobsDto[]>([]);

  protected readonly machineCards = computed<MachineCard[]>(() =>
    this.machines().map(m => ({
      id: m.id,
      name: m.serialNumber,
      modelNumber: m.modelNumber,
      jobs: m.jobs.map(j => ({
        id: j.id,
        partNumber: j.partNumber,
        dueDate: j.dueDate,
        lotNumber: j.lotNumber
      }))
    }))
  );

  ngOnInit(): void {
    this.loadMachines();
  }

  protected loadMachines(): void {
    this.loading.set(true);
    this.machinesApi.listWithJobs().subscribe({
      next: (data) => {
        this.machines.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.toast.error('Failed to load machines');
        this.loading.set(false);
      }
    });
  }
}
