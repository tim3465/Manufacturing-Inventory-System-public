import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { MachinesApi } from '../../../../core/api/machines.api';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-inactivate-machine-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './inactivate-machine-modal.component.html',
  styleUrl: './inactivate-machine-modal.component.css'
})
export class InactivateMachineModalComponent {
  private readonly machinesApi = inject(MachinesApi);
  private readonly toast = inject(ToastService);

  @Input({ required: true }) machineId!: number;
  @Input({ required: true }) serialNumber!: string;
  @Input({ required: true }) modelNumber!: string;

  @Output() closed = new EventEmitter<void>();
  @Output() inactivated = new EventEmitter<void>();

  protected readonly submitting = signal<boolean>(false);

  protected onCancel(): void {
    if (this.submitting()) return;
    this.closed.emit();
  }

  protected onConfirm(): void {
    if (this.submitting()) return;

    this.submitting.set(true);
    this.machinesApi.inactivate(this.machineId).subscribe({
      next: () => {
        this.toast.success('Machine inactivated');
        this.inactivated.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to inactivate machine');
        this.submitting.set(false);
      }
    });
  }
}
