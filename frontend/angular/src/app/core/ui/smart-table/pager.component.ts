import { Component, EventEmitter, Input, Output } from '@angular/core';

/**
 * Reusable pager for server-driven tables.
 *
 * Usage:
 *   <app-pager
 *     [currentPage]="table.currentPage()"
 *     [totalPages]="totalPages()"
 *     [totalCount]="totalCount()"
 *     (pageChange)="table.goToPage($event)"
 *     [pageSize]="table.pageSize()"
 *     [pageSizes]="pageSizes"
 *     (pageSizeChange)="table.onPageSizeChange($event)"
 *   />
 */
@Component({
  selector: 'app-pager',
  standalone: true,
  templateUrl: './pager.component.html'
})
export class PagerComponent {
  @Input({ required: true }) currentPage!: number;
  @Input({ required: true }) totalPages!: number;
  @Input({ required: true }) totalCount!: number;
  @Output() readonly pageChange = new EventEmitter<number>();

  @Input() pageSize?: number;
  @Input() pageSizes?: number[];
  @Output() readonly pageSizeChange = new EventEmitter<number>();

  protected prev(): void {
    this.pageChange.emit(this.currentPage - 1);
  }

  protected next(): void {
    this.pageChange.emit(this.currentPage + 1);
  }

  protected onPageSizeChange(event: Event): void {
    this.pageSizeChange.emit(Number((event.target as HTMLSelectElement).value));
  }
}
