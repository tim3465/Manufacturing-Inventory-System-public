import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { MaterialsApi } from '../../../core/api/materials.api';
import { MaterialDto, MaterialSearchRequestDto, MaterialSearchResultDto } from '../../../core/dtos/materials';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { PagerComponent, SmartTableState } from '../../../core/ui/smart-table';
import { EditMaterialModalComponent } from './edit-material-modal/edit-material-modal.component';

@Component({
  selector: 'app-receive-material-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PagerComponent, EditMaterialModalComponent],
  templateUrl: './receive-material.page.html',
  styleUrl: './receive-material.page.css'
})
export class ReceiveMaterialPageComponent {
  private readonly materialsApi = inject(MaterialsApi);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);

  protected readonly table = new SmartTableState({
    defaultSortColumn: 'HeatNumber',
    defaultSortDirection: 'asc',
    pageSize: 10
  });

  protected readonly pageSizes = [5, 10, 25, 100];

  protected readonly filterForm = this.fb.nonNullable.group({
    heatNumber: [''],
    materialName: ['']
  });

  private readonly searchResult = signal<MaterialSearchResultDto | null>(null);

  protected readonly rows = computed(() => this.searchResult()?.items ?? []);
  protected readonly totalCount = computed(() => this.searchResult()?.totalCount ?? 0);
  protected readonly totalPages = computed(() => Math.ceil(this.totalCount() / this.table.pageSize()) || 1);

  protected readonly isEditOpen = signal<boolean>(false);
  protected readonly selectedMaterial = signal<MaterialDto | null>(null);

  constructor() {
    this.filterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      this.table.resetPage();
      this.executeSearch();
    });

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

    const request: MaterialSearchRequestDto = {
      sortColumn: this.table.sortColumn(),
      sortDirection: this.table.sortDirection(),
      page: this.table.currentPage(),
      pageSize: this.table.pageSize()
    };

    if (f.heatNumber?.trim()) {
      request.heatNumber = f.heatNumber.trim();
    }
    if (f.materialName?.trim()) {
      request.materialName = f.materialName.trim();
    }

    this.materialsApi.search(request).subscribe({
      next: (result) => {
        this.searchResult.set(result);
        this.table.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load materials';
        this.table.error.set(message);
        this.toast.error(message);
        this.table.loading.set(false);
      }
    });
  }

  protected openEdit(material: MaterialDto): void {
    this.selectedMaterial.set(material);
    this.isEditOpen.set(true);
  }

  protected closeEdit(): void {
    this.isEditOpen.set(false);
    this.selectedMaterial.set(null);
  }
}
