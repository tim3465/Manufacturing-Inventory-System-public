import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { ShiftsApi } from '../../../core/api/shifts.api';
import { ShiftLogSearchResultDto } from '../../../core/dtos/shifts/shift-log-search-result.dto';
import { RunningShiftDto } from '../../../core/dtos/shifts/running-shift.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../../core/ui/smart-table';
import { RunningShiftFormComponent } from './running-shift-form/running-shift-form.component';

@Component({
  selector: 'app-shifts-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PagerComponent, RunningShiftFormComponent],
  templateUrl: './shifts.page.html',
  styleUrl: './shifts.page.css'
})
export class ShiftsPageComponent {
  private readonly shiftsApi = inject(ShiftsApi);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);

  protected readonly activeTab = signal<'running' | 'logs'>('running');
  protected readonly runningShifts = signal<RunningShiftDto[]>([]);
  protected readonly expandedShiftId = signal<number | null>(null);
  protected readonly runningLoading = signal(true);

  protected readonly logsTable = new SmartTableState({
    defaultSortColumn: 'StartTime',
    defaultSortDirection: 'desc',
    pageSize: 10
  });

  protected readonly pageSizes = [5, 10, 25, 100];

  protected readonly logsFilterForm = this.fb.nonNullable.group({
    machineName: [''],
    jobNumber: [''],
    partNumber: [''],
    startTimeFrom: [''],
    startTimeTo: [''],
    stopTimeFrom: [''],
    stopTimeTo: ['']
  });

  private readonly logsSearchResult = signal<ShiftLogSearchResultDto | null>(null);

  protected readonly logRows = computed(() => this.logsSearchResult()?.items ?? []);
  protected readonly logsTotalCount = computed(() => this.logsSearchResult()?.totalCount ?? 0);
  protected readonly logsTotalPages = computed(() =>
    Math.ceil(this.logsTotalCount() / this.logsTable.pageSize()) || 1
  );

  constructor() {
    // Load running shifts immediately
    this.reloadRunning();

    // Filter form changes → reset page + search
    this.logsFilterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.logsTable.resetPage();
      this.executeLogsSearch();
    });

    // Sort / page / page-size changes → search (also handles initial load)
    effect(() => {
      this.logsTable.sortColumn();
      this.logsTable.sortDirection();
      this.logsTable.currentPage();
      this.logsTable.pageSize();

      untracked(() => this.executeLogsSearch());
    });
  }

  protected executeLogsSearch(): void {
    this.logsTable.loading.set(true);
    this.logsTable.error.set(null);

    const f = this.logsFilterForm.getRawValue();

    const request = {
      sortColumn: this.logsTable.sortColumn(),
      sortDirection: this.logsTable.sortDirection(),
      page: this.logsTable.currentPage(),
      pageSize: this.logsTable.pageSize(),
      ...(f.machineName?.trim() ? { machineName: f.machineName.trim() } : {}),
      ...(f.jobNumber?.trim() ? { jobNumber: f.jobNumber.trim() } : {}),
      ...(f.partNumber?.trim() ? { partNumber: f.partNumber.trim() } : {}),
      ...(f.startTimeFrom ? { startTimeFrom: f.startTimeFrom } : {}),
      ...(f.startTimeTo ? { startTimeTo: f.startTimeTo } : {}),
      ...(f.stopTimeFrom ? { stopTimeFrom: f.stopTimeFrom } : {}),
      ...(f.stopTimeTo ? { stopTimeTo: f.stopTimeTo } : {})
    };

    this.shiftsApi.searchMyLogs(request).subscribe({
      next: (result) => {
        this.logsSearchResult.set(result);
        this.logsTable.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load shift logs';
        this.logsTable.error.set(message);
        this.toast.error(message);
        this.logsTable.loading.set(false);
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
    this.executeLogsSearch();
  }

  private reloadRunning(): void {
    this.runningLoading.set(true);
    this.shiftsApi.listRunning().subscribe({
      next: (data) => {
        this.runningShifts.set(data);
        this.runningLoading.set(false);
      },
      error: () => {
        this.toast.error('Failed to reload running shifts');
        this.runningLoading.set(false);
      }
    });
  }
}
