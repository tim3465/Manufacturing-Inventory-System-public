import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { UsersApi } from '../../../../core/api/users.api';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-inactivate-user-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './inactivate-user-modal.component.html',
  styleUrl: './inactivate-user-modal.component.css'
})
export class InactivateUserModalComponent {
  private readonly usersApi = inject(UsersApi);
  private readonly toast = inject(ToastService);

  @Input({ required: true }) userId!: number;
  @Input({ required: true }) fullName!: string;
  @Input({ required: true }) email!: string;

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
    this.usersApi.inactivate(this.userId).subscribe({
      next: () => {
        this.toast.success('User inactivated');
        this.inactivated.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to inactivate user');
        this.submitting.set(false);
      }
    });
  }
}


