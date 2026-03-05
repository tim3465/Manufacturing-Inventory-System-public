import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { MaterialsApi } from '../../../core/api/materials.api';
import { MaterialDto } from '../../../core/dtos/materials';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { EditMaterialModalComponent } from './edit-material-modal/edit-material-modal.component';

@Component({
  selector: 'app-receive-material-page',
  standalone: true,
  imports: [CommonModule, EditMaterialModalComponent],
  templateUrl: './receive-material.page.html',
  styleUrl: './receive-material.page.css'
})
export class ReceiveMaterialPageComponent implements OnInit {
  private readonly materialsApi = inject(MaterialsApi);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal<boolean>(true);
  protected readonly error = signal<string | null>(null);
  protected readonly materials = signal<MaterialDto[]>([]);

  protected readonly isEditOpen = signal<boolean>(false);
  protected readonly selectedMaterial = signal<MaterialDto | null>(null);

  ngOnInit(): void {
    this.loadMaterials();
  }

  protected loadMaterials(): void {
    this.loading.set(true);
    this.error.set(null);

    this.materialsApi.listActive().subscribe({
      next: (materials) => {
        this.materials.set(materials);
        this.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load materials';
        this.error.set(message);
        this.toast.error(message);
        this.loading.set(false);
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
