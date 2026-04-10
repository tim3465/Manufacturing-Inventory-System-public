import { Component, OnInit, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { ShiftsApi } from '../../../../../core/api/shifts.api';
import { ShiftProductionSearchRequestDto } from '../../../../../core/dtos/shifts/shift-production-search-request.dto';
import { ShiftProductionSearchResultDto } from '../../../../../core/dtos/shifts/shift-production-search-result.dto';
import { ToastService } from '../../../../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../../../../core/ui/smart-table';

@Component({
  selector: 'app-production-shifts-tab',
  standalone: true,
  imports: [ReactiveFormsModule, PagerComponent],
  templateUrl: './production-shifts-tab.component.html',
  styleUrl: './production-shifts-tab.component.css'
})
export class ProductionShiftsTabComponent implements OnInit {
  private readonly shiftsApi = inject(ShiftsApi);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);

  protected readonly shiftsTable = new SmartTableState({
    defaultSortColumn: 'StartTime',
    defaultSortDirection: 'desc',
    pageSize: 10
  });

  protected readonly shiftsFilterForm = this.fb.nonNullable.group({
    operatorName: [''],
    jobNumber: [''],
    startTimeFrom: [''],
    startTimeTo: [''],
    stopTimeFrom: [''],
    stopTimeTo: ['']
  });

  private readonly shiftsSearchResult = signal<ShiftProductionSearchResultDto | null>(null);

  protected readonly shiftRows = computed(() => this.shiftsSearchResult()?.items ?? []);
  protected readonly shiftsTotalCount = computed(() => this.shiftsSearchResult()?.totalCount ?? 0);
  protected readonly shiftsTotalPages = computed(() => Math.ceil(this.shiftsTotalCount() / this.shiftsTable.pageSize()) || 1);

  protected readonly pageSizes = [5, 10, 25, 100];

  constructor() {
    this.shiftsFilterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.shiftsTable.resetPage();
      this.executeShiftsSearch();
    });

    effect(() => {
      this.shiftsTable.sortColumn();
      this.shiftsTable.sortDirection();
      this.shiftsTable.currentPage();
      this.shiftsTable.pageSize();

      untracked(() => {
        this.executeShiftsSearch();
      });
    });
  }

  ngOnInit(): void {
    this.executeShiftsSearch();
  }

  refresh(): void {
    this.executeShiftsSearch();
  }

  protected formatStartTime(startTime: string): string {
    return new Date(startTime).toLocaleString();
  }

  protected formatStopTime(stopTime: string | null): string {
    if (!stopTime) return 'In Progress';
    return new Date(stopTime).toLocaleString();
  }

  private executeShiftsSearch(): void {
    this.shiftsTable.loading.set(true);
    this.shiftsTable.error.set(null);

    const f = this.shiftsFilterForm.getRawValue();

    const request: ShiftProductionSearchRequestDto = {
      sortColumn: this.shiftsTable.sortColumn(),
      sortDirection: this.shiftsTable.sortDirection(),
      page: this.shiftsTable.currentPage(),
      pageSize: this.shiftsTable.pageSize()
    };

    if (f.operatorName?.trim()) {
      request.operatorName = f.operatorName.trim();
    }
    if (f.jobNumber?.trim()) {
      request.jobNumber = f.jobNumber.trim();
    }
    if (f.startTimeFrom?.trim()) {
      request.startTimeFrom = f.startTimeFrom.trim();
    }
    if (f.startTimeTo?.trim()) {
      request.startTimeTo = f.startTimeTo.trim();
    }
    if (f.stopTimeFrom?.trim()) {
      request.stopTimeFrom = f.stopTimeFrom.trim();
    }
    if (f.stopTimeTo?.trim()) {
      request.stopTimeTo = f.stopTimeTo.trim();
    }

    this.shiftsApi.searchProduction(request).subscribe({
      next: (result) => {
        this.shiftsSearchResult.set(result);
        this.shiftsTable.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load shifts';
        this.shiftsTable.error.set(message);
        this.toast.error(message);
        this.shiftsTable.loading.set(false);
      }
    });
  }
}
