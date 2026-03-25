import { signal, WritableSignal } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

export interface SmartTableStateOptions {
  defaultSortColumn?: string;
  defaultSortDirection?: 'asc' | 'desc';
  pageSize?: number;
  debounceMs?: number;
}

/**
 * Reusable state for server-driven tables.
 *
 * Holds sort, page, loading, and error signals plus debounced filter-change
 * plumbing. Instantiate one per page component.
 *
 * Usage:
 *   protected readonly table = new SmartTableState({ defaultSortColumn: 'Name', defaultSortDirection: 'asc' });
 *
 *   constructor() {
 *     // Debounced text filter → reset page + search
 *     this.table.debouncedFilterChange$.subscribe(() => {
 *       this.table.resetPage();
 *       this.executeSearch();
 *     });
 *
 *     // Sort / page changes → search (also handles initial load)
 *     effect(() => {
 *       this.table.sortColumn();
 *       this.table.sortDirection();
 *       this.table.currentPage();
 *       untracked(() => this.executeSearch());
 *     });
 *   }
 */
export class SmartTableState {
  readonly sortColumn: WritableSignal<string>;
  readonly sortDirection: WritableSignal<'asc' | 'desc'>;
  readonly currentPage: WritableSignal<number>;
  readonly pageSize: WritableSignal<number>;
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  private readonly filterInput$ = new Subject<void>();

  /** Emits after debounce delay — subscribe to reset page and trigger a search. */
  readonly debouncedFilterChange$: Observable<void>;

  constructor(options: SmartTableStateOptions = {}) {
    this.sortColumn = signal(options.defaultSortColumn ?? 'id');
    this.sortDirection = signal(options.defaultSortDirection ?? 'asc');
    this.currentPage = signal(1);
    this.pageSize = signal(options.pageSize ?? 10);
    this.debouncedFilterChange$ = this.filterInput$.pipe(debounceTime(options.debounceMs ?? 300));
  }

  /** Call from template (input) or (change) events on filter inputs. */
  onFilterInput(): void {
    this.filterInput$.next();
  }

  /** Toggle sort direction if same column; otherwise switch column and reset to asc. Resets to page 1. */
  onSortColumn(col: string): void {
    if (this.sortColumn() === col) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(col);
      this.sortDirection.set('asc');
    }
    this.currentPage.set(1);
  }

  /** Navigate to the given page number. */
  goToPage(page: number): void {
    this.currentPage.set(page);
  }

  /** Change the page size and reset to page 1. */
  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.currentPage.set(1);
  }

  /** Returns ↑ or ↓ if the column is the active sort, otherwise empty string. */
  sortIndicator(col: string): string {
    if (this.sortColumn() !== col) return '';
    return this.sortDirection() === 'asc' ? ' \u2191' : ' \u2193';
  }

  /** Reset to the first page — call before triggering a filter-driven search. */
  resetPage(): void {
    this.currentPage.set(1);
  }
}
