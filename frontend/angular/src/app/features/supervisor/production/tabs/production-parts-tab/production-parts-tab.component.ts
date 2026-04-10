import { Component, OnInit, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { PartsApi } from '../../../../../core/api/parts.api';
import { PartSearchRequestDto } from '../../../../../core/dtos/parts/part-search-request.dto';
import { PartSearchResultDto } from '../../../../../core/dtos/parts/part-search-result.dto';
import { ToastService } from '../../../../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../../../../core/ui/smart-table';

@Component({
  selector: 'app-production-parts-tab',
  standalone: true,
  imports: [ReactiveFormsModule, PagerComponent],
  templateUrl: './production-parts-tab.component.html',
  styleUrl: './production-parts-tab.component.css'
})
export class ProductionPartsTabComponent implements OnInit {
  private readonly partsApi = inject(PartsApi);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);

  protected readonly partsTable = new SmartTableState({
    defaultSortColumn: 'PartName',
    defaultSortDirection: 'asc',
    pageSize: 10
  });

  protected readonly partsFilterForm = this.fb.nonNullable.group({
    partName: [''],
    partNumber: ['']
  });

  private readonly partsSearchResult = signal<PartSearchResultDto | null>(null);

  protected readonly partRows = computed(() => this.partsSearchResult()?.items ?? []);
  protected readonly partsTotalCount = computed(() => this.partsSearchResult()?.totalCount ?? 0);
  protected readonly partsTotalPages = computed(() => Math.ceil(this.partsTotalCount() / this.partsTable.pageSize()) || 1);

  protected readonly pageSizes = [5, 10, 25, 100];

  constructor() {
    this.partsFilterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.partsTable.resetPage();
      this.executePartsSearch();
    });

    effect(() => {
      this.partsTable.sortColumn();
      this.partsTable.sortDirection();
      this.partsTable.currentPage();
      this.partsTable.pageSize();

      untracked(() => {
        this.executePartsSearch();
      });
    });
  }

  ngOnInit(): void {
    this.executePartsSearch();
  }

  refresh(): void {
    this.executePartsSearch();
  }

  protected formatCycleTime(time: string): string {
    return time ?? '—';
  }

  private executePartsSearch(): void {
    this.partsTable.loading.set(true);
    this.partsTable.error.set(null);

    const f = this.partsFilterForm.getRawValue();

    const request: PartSearchRequestDto = {
      sortColumn: this.partsTable.sortColumn(),
      sortDirection: this.partsTable.sortDirection(),
      page: this.partsTable.currentPage(),
      pageSize: this.partsTable.pageSize()
    };

    if (f.partName?.trim()) {
      request.partName = f.partName.trim();
    }
    if (f.partNumber?.trim()) {
      request.partNumber = f.partNumber.trim();
    }

    this.partsApi.search(request).subscribe({
      next: (result) => {
        this.partsSearchResult.set(result);
        this.partsTable.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load parts';
        this.partsTable.error.set(message);
        this.toast.error(message);
        this.partsTable.loading.set(false);
      }
    });
  }
}
