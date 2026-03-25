import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { StockLotsApi } from '../../../core/api/stock-lots.api';
import { StockLotSearchRequestDto } from '../../../core/dtos/stock-lots/stock-lot-search-request.dto';
import { StockLotSearchResultDto } from '../../../core/dtos/stock-lots/stock-lot-search-result.dto';
import { StockLotSummaryDto } from '../../../core/dtos/stock-lots/stock-lot-summary.dto';
import { STOCK_LOT_CONDITION_LABELS, STOCK_LOT_CONDITIONS, StockLotCondition } from '../../../core/dtos/shipping-receiving';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../../core/ui/smart-table';
import { ReceiveShipmentModalComponent } from './receive-shipment-modal/receive-shipment-modal.component';
import { AdjustBarsModalComponent } from './adjust-bars-modal/adjust-bars-modal.component';

@Component({
  selector: 'app-inventory-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PagerComponent, ReceiveShipmentModalComponent, AdjustBarsModalComponent],
  templateUrl: './inventory.page.html',
  styleUrl: './inventory.page.css'
})
export class InventoryPageComponent {
  private readonly stockLotsApi = inject(StockLotsApi);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);

  protected readonly table = new SmartTableState({
    defaultSortColumn: 'CheckedInDateTime',
    defaultSortDirection: 'desc',
    pageSize: 25
  });

  protected readonly pageSizes = [5, 10, 25, 100];

  protected readonly filterForm = this.fb.nonNullable.group({
    lotNumber: [''],
    diameter: [''],
    checkedInFrom: [''],
    checkedInTo: [''],
    condition: ['']
  });

  private readonly searchResult = signal<StockLotSearchResultDto | null>(null);

  protected readonly rows = computed(() => this.searchResult()?.items ?? []);
  protected readonly totalCount = computed(() => this.searchResult()?.totalCount ?? 0);
  protected readonly totalPages = computed(() => Math.ceil(this.totalCount() / this.table.pageSize()) || 1);

  protected readonly isReceiveShipmentOpen = signal<boolean>(false);
  protected readonly isAdjustBarsOpen = signal<boolean>(false);
  protected readonly selectedLotForAdjustment = signal<StockLotSummaryDto | null>(null);

  protected readonly conditionLabels = STOCK_LOT_CONDITION_LABELS;
  protected readonly conditions = STOCK_LOT_CONDITIONS;

  constructor() {
    // Debounced text/date/select filter changes → reset to page 1 then search
    this.table.debouncedFilterChange$.subscribe(() => {
      this.table.resetPage();
      this.executeSearch();
    });

    // Sort, page, and page-size changes → search (also handles initial load)
    effect(() => {
      this.table.sortColumn();
      this.table.sortDirection();
      this.table.currentPage();
      this.table.pageSize();

      untracked(() => this.executeSearch());
    });
  }

  protected executeSearch(): void {
    this.table.loading.set(true);
    this.table.error.set(null);

    const f = this.filterForm.getRawValue();

    const request: StockLotSearchRequestDto = {
      sortColumn: this.table.sortColumn(),
      sortDirection: this.table.sortDirection(),
      page: this.table.currentPage(),
      pageSize: this.table.pageSize()
    };

    if (f.lotNumber?.trim()) {
      request.lotNumber = f.lotNumber.trim();
    }
    if (f.diameter) {
      request.diameterExact = Number(f.diameter);
    }
    if (f.checkedInFrom) {
      request.checkedInFrom = f.checkedInFrom;
    }
    if (f.checkedInTo) {
      request.checkedInTo = f.checkedInTo;
    }
    if (f.condition) {
      request.condition = Number(f.condition) as StockLotCondition;
    }

    this.stockLotsApi.search(request).subscribe({
      next: (result) => {
        this.searchResult.set(result);
        this.table.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load inventory';
        this.table.error.set(message);
        this.toast.error(message);
        this.table.loading.set(false);
      }
    });
  }

  protected openReceiveShipment(): void {
    this.isReceiveShipmentOpen.set(true);
  }

  protected closeReceiveShipment(): void {
    this.isReceiveShipmentOpen.set(false);
  }

  protected openAdjustBars(lot: StockLotSummaryDto): void {
    this.selectedLotForAdjustment.set(lot);
    this.isAdjustBarsOpen.set(true);
  }

  protected closeAdjustBars(): void {
    this.isAdjustBarsOpen.set(false);
    this.selectedLotForAdjustment.set(null);
  }

  protected conditionLabel(condition: StockLotCondition): string {
    return this.conditionLabels[condition] ?? String(condition);
  }
}
