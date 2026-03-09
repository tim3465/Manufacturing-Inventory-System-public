import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MachinesApi } from '../../../core/api/machines.api';
import { MachineDto } from '../../../core/dtos/machines/machine.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { AddMachineModalComponent } from './add-machine-modal/add-machine-modal.component';
import { InactivateMachineModalComponent } from './inactivate-machine-modal/inactivate-machine-modal.component';
import { ActivateMachineModalComponent } from './activate-machine-modal/activate-machine-modal.component';

interface MachineRow {
  id: number;
  serialNumber: string;
  modelNumber: string;
  isActive: boolean;
}

@Component({
  selector: 'app-machines-page',
  standalone: true,
  imports: [
    CommonModule,
    AddMachineModalComponent,
    InactivateMachineModalComponent,
    ActivateMachineModalComponent
  ],
  templateUrl: './machines.page.html',
  styleUrl: './machines.page.css'
})
export class MachinesPageComponent implements OnInit {
  private readonly machinesApi = inject(MachinesApi);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal<boolean>(true);
  protected readonly error = signal<string | null>(null);
  protected readonly machines = signal<MachineDto[]>([]);
  protected readonly isAddMachineOpen = signal<boolean>(false);
  protected readonly isInactivateOpen = signal<boolean>(false);
  protected readonly isActivateOpen = signal<boolean>(false);
  protected readonly selectedMachine = signal<MachineRow | null>(null);

  protected readonly machineRows = computed<MachineRow[]>(() =>
    this.machines().map((m) => ({
      id: m.id,
      serialNumber: m.serialNumber,
      modelNumber: m.modelNumber,
      isActive: !m.inactivatedDateTime
    }))
  );

  ngOnInit(): void {
    this.loadMachines();
  }

  protected openAddMachine(): void {
    this.isAddMachineOpen.set(true);
  }

  protected closeAddMachine(): void {
    this.isAddMachineOpen.set(false);
  }

  protected openInactivate(machine: MachineRow): void {
    this.selectedMachine.set(machine);
    this.isInactivateOpen.set(true);
  }

  protected closeInactivate(): void {
    this.isInactivateOpen.set(false);
    this.selectedMachine.set(null);
  }

  protected openActivate(machine: MachineRow): void {
    this.selectedMachine.set(machine);
    this.isActivateOpen.set(true);
  }

  protected closeActivate(): void {
    this.isActivateOpen.set(false);
    this.selectedMachine.set(null);
  }

  protected loadMachines(): void {
    this.loading.set(true);
    this.error.set(null);

    this.machinesApi.listAll().subscribe({
      next: (machines) => {
        this.machines.set(machines);
        this.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load machines';
        this.error.set(message);
        this.toast.error(message);
        this.loading.set(false);
      }
    });
  }
}
