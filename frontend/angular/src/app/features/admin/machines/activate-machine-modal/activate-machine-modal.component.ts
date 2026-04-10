import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { MachinesApi } from '../../../../core/api/machines.api';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-activate-machine-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './activate-machine-modal.component.html',
  styleUrl: './activate-machine-modal.component.css'
})
export class ActivateMachineModalComponent {
  private readonly machinesApi = inject(MachinesApi);
  private readonly toast = inject(ToastService);

  @Input({ required: true }) machineId!: number;
  @Input({ required: true }) serialNumber!: string;
  @Input({ required: true }) modelNumber!: string;

  @Output() closed = new EventEmitter<void>();
  @Output() activated = new EventEmitter<void>();

  protected readonly submitting = signal<boolean>(false);

  protected onCancel(): void {
    if (this.submitting()) return;
    this.closed.emit();
  }

  protected onConfirm(): void {
    if (this.submitting()) return;

    this.submitting.set(true);
    this.machinesApi.activate(this.machineId).subscribe({
      next: () => {
        this.toast.success('Machine activated');
        this.activated.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to activate machine');
        this.submitting.set(false);
      }
    });
  }
}
