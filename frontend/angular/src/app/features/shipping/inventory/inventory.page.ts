import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { StockLotsApi } from '../../../core/api/stock-lots.api';
import { StockLotSearchRequestDto } from '../../../core/dtos/stock-lots/stock-lot-search-request.dto';
import { StockLotSearchResultDto } from '../../../core/dtos/stock-lots/stock-lot-search-result.dto';
import { StockLotSummaryDto } from '../../../core/dtos/stock-lots/stock-lot-summary.dto';
import { STOCK_LOT_CONDITION_LABELS, StockLotCondition } from '../../../core/dtos/shipping-receiving';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { ReceiveShipmentModalComponent } from './receive-shipment-modal/receive-shipment-modal.component';
import { AdjustBarsModalComponent } from './adjust-bars-modal/adjust-bars-modal.component';

@Component({
  selector: 'app-inventory-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ReceiveShipmentModalComponent, AdjustBarsModalComponent],
  templateUrl: './inventory.page.html',
  styleUrl: './inventory.page.css'
})
export class InventoryPageComponent {
  private readonly stockLotsApi = inject(StockLotsApi);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);

  protected readonly filterForm = this.fb.nonNullable.group({
    lotNumber: [''],
    diameter: [''],
    checkedInFrom: [''],
    checkedInTo: ['']
  });

  protected readonly sortColumn = signal<string>('CheckedInDateTime');
  protected readonly sortDirection = signal<'asc' | 'desc'>('desc');
  protected readonly currentPage = signal<number>(1);
  protected readonly pageSize = signal<number>(25);
  protected readonly searchResult = signal<StockLotSearchResultDto | null>(null);
  protected readonly loading = signal<boolean>(false);
  protected readonly error = signal<string | null>(null);

  protected readonly rows = computed(() => this.searchResult()?.items ?? []);
  protected readonly totalCount = computed(() => this.searchResult()?.totalCount ?? 0);
  protected readonly totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()) || 1);

  protected readonly isReceiveShipmentOpen = signal<boolean>(false);
  protected readonly isAdjustBarsOpen = signal<boolean>(false);
  protected readonly selectedLotForAdjustment = signal<StockLotSummaryDto | null>(null);

  protected readonly conditionLabels = STOCK_LOT_CONDITION_LABELS;

  private readonly textFilterChanged$ = new Subject<void>();

  constructor() {
    // Debounce text filter changes
    this.textFilterChanged$
      .pipe(debounceTime(300))
      .subscribe(() => {
        this.currentPage.set(1);
        this.executeSearch();
      });

    // React to sort/page changes (also handles initial load)
    effect(() => {
      // Read signals to register as dependencies
      this.sortColumn();
      this.sortDirection();
      this.currentPage();

      untracked(() => this.executeSearch());
    });
  }

  protected executeSearch(): void {
    this.loading.set(true);
    this.error.set(null);

    const f = this.filterForm.getRawValue();

    const request: StockLotSearchRequestDto = {
      sortColumn: this.sortColumn(),
      sortDirection: this.sortDirection(),
      page: this.currentPage(),
      pageSize: this.pageSize()
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

    this.stockLotsApi.search(request).subscribe({
      next: (result) => {
        this.searchResult.set(result);
        this.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load inventory';
        this.error.set(message);
        this.toast.error(message);
        this.loading.set(false);
      }
    });
  }

  protected onFilterInput(): void {
    this.textFilterChanged$.next();
  }

  protected onSortColumn(col: string): void {
    if (this.sortColumn() === col) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(col);
      this.sortDirection.set('asc');
    }
    this.currentPage.set(1);
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
  }

  protected sortIndicator(col: string): string {
    if (this.sortColumn() !== col) return '';
    return this.sortDirection() === 'asc' ? ' \u2191' : ' \u2193';
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
