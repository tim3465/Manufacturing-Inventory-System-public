import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MachinesApi } from '../../core/api/machines.api';
import { MachineWithJobsDto } from '../../core/dtos/machines/machine-with-jobs.dto';
import { ToastService } from '../../core/ui/toast/toast.service';
import { AuthService } from '../../core/auth/auth.service';
import { StartJobModalComponent } from './start-job-modal/start-job-modal.component';
import { StartShiftModalComponent } from './start-shift-modal/start-shift-modal.component';

interface JobRow {
  id: number;
  partNumber: string;
  dueDate: string;
  lotNumber: string | null;
  isActive: boolean;
  barsInJob: number;
  barAmountPlanned: number;
  hasLotNumber: boolean;
  machineHasActiveJob: boolean;
  runningShiftId: number | null;
  runningShiftOperatorId: number | null;
}

interface MachineCard {
  id: number;
  name: string;
  modelNumber: string;
  jobs: JobRow[];
  cardState: 'my-running' | 'other-running' | 'available' | 'default';
}

@Component({
  selector: 'app-machinist-machines-page',
  standalone: true,
  imports: [CommonModule, StartJobModalComponent, StartShiftModalComponent],
  templateUrl: './machines.page.html',
  styleUrl: './machines.page.css'
})
export class MachinistMachinesPageComponent implements OnInit {
  private readonly machinesApi = inject(MachinesApi);
  private readonly toast = inject(ToastService);
  private readonly auth = inject(AuthService);

  protected readonly loading = signal<boolean>(true);
  protected readonly machines = signal<MachineWithJobsDto[]>([]);
  protected readonly selectedJob = signal<JobRow | null>(null);
  protected readonly showStartModal = signal<boolean>(false);
  protected readonly selectedJobId = signal<number | null>(null);
  protected readonly selectedMachineSerial = signal<string | null>(null);
  protected readonly selectedPartNumber = signal<string | null>(null);
  protected readonly showStartShiftModal = signal<boolean>(false);

  protected readonly machineCards = computed<MachineCard[]>(() =>
    this.machines().map(m => {
      const currentUserId = this.auth.getUserId();
      const hasRunningShift = m.jobs.some(j => j.runningShiftId != null);
      const isMyShift = m.jobs.some(j => j.runningShiftOperatorId === currentUserId);
      const hasActiveJob = m.jobs.some(j => j.startedDateTime != null);

      let cardState: 'my-running' | 'other-running' | 'available' | 'default';
      if (hasRunningShift && isMyShift) cardState = 'my-running';
      else if (hasRunningShift) cardState = 'other-running';
      else if (hasActiveJob) cardState = 'available';
      else cardState = 'default';

      return {
        id: m.id,
        name: m.serialNumber,
        modelNumber: m.modelNumber,
        cardState,
        jobs: m.jobs.map(j => ({
          id: j.id,
          partNumber: j.partNumber,
          dueDate: j.dueDate,
          lotNumber: j.lotNumber,
          isActive: j.startedDateTime != null,
          barsInJob: j.barsInJob,
          barAmountPlanned: j.barAmountPlanned,
          hasLotNumber: j.lotNumber !== null,
          machineHasActiveJob: hasActiveJob,
          runningShiftId: j.runningShiftId,
          runningShiftOperatorId: j.runningShiftOperatorId
        }))
      };
    })
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

  protected onJobRowClick(job: JobRow): void {
    if (job.isActive || !job.hasLotNumber || job.machineHasActiveJob) return;
    this.selectedJob.set(job);
    this.showStartModal.set(true);
  }

  protected onJobStarted(): void {
    this.showStartModal.set(false);
    this.loadMachines();
  }

  protected openStartShift(job: JobRow, machineSerial: string): void {
    this.selectedJobId.set(job.id);
    this.selectedMachineSerial.set(machineSerial);
    this.selectedPartNumber.set(job.partNumber);
    this.showStartShiftModal.set(true);
  }

  protected onShiftStarted(): void {
    this.showStartShiftModal.set(false);
    this.selectedJobId.set(null);
    this.selectedMachineSerial.set(null);
    this.selectedPartNumber.set(null);
    this.loadMachines();
  }
}
