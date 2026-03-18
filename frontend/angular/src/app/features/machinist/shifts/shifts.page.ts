import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ShiftsApi } from '../../../core/api/shifts.api';
import { RunningShiftDto } from '../../../core/dtos/shifts/running-shift.dto';
import { ShiftLogDto } from '../../../core/dtos/shifts/shift-log.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { RunningShiftFormComponent } from './running-shift-form/running-shift-form.component';

@Component({
  selector: 'app-shifts-page',
  standalone: true,
  imports: [CommonModule, RunningShiftFormComponent],
  templateUrl: './shifts.page.html',
  styleUrl: './shifts.page.css'
})
export class ShiftsPageComponent implements OnInit {
  private readonly shiftsApi = inject(ShiftsApi);
  private readonly toast = inject(ToastService);

  protected readonly activeTab = signal<'running' | 'logs'>('running');
  protected readonly loading = signal(true);
  protected readonly runningShifts = signal<RunningShiftDto[]>([]);
  protected readonly shiftLogs = signal<ShiftLogDto[]>([]);
  protected readonly expandedShiftId = signal<number | null>(null);

  ngOnInit(): void {
    this.loadAll();
  }

  private loadAll(): void {
    this.loading.set(true);
    let runningDone = false;
    let logsDone = false;

    const checkDone = () => {
      if (runningDone && logsDone) {
        this.loading.set(false);
      }
    };

    this.shiftsApi.listRunning().subscribe({
      next: (data) => {
        this.runningShifts.set(data);
        runningDone = true;
        checkDone();
      },
      error: () => {
        this.toast.error('Failed to load running shifts');
        runningDone = true;
        checkDone();
      }
    });

    this.shiftsApi.listMyLogs().subscribe({
      next: (data) => {
        this.shiftLogs.set(data);
        logsDone = true;
        checkDone();
      },
      error: () => {
        this.toast.error('Failed to load shift logs');
        logsDone = true;
        checkDone();
      }
    });
  }

  protected setTab(tab: 'running' | 'logs'): void {
    this.activeTab.set(tab);
  }

  protected toggleExpand(shiftId: number): void {
    if (this.expandedShiftId() === shiftId) {
      this.expandedShiftId.set(null);
    } else {
      this.expandedShiftId.set(shiftId);
    }
  }

  protected onShiftSaved(): void {
    this.reloadRunning();
  }

  protected onShiftClosed(): void {
    this.expandedShiftId.set(null);
    this.reloadRunning();
    this.reloadLogs();
  }

  private reloadRunning(): void {
    this.shiftsApi.listRunning().subscribe({
      next: (data) => this.runningShifts.set(data),
      error: () => this.toast.error('Failed to reload running shifts')
    });
  }

  private reloadLogs(): void {
    this.shiftsApi.listMyLogs().subscribe({
      next: (data) => this.shiftLogs.set(data),
      error: () => this.toast.error('Failed to reload shift logs')
    });
  }
}
