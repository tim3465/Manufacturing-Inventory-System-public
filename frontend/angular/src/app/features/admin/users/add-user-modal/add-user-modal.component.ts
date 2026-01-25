import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UsersApi } from '../../../../core/api/users.api';
import { CreateUserRequestDto } from '../../../../core/dtos/users';
import { ALL_ROLES, ROLE_LABELS, Role } from '../../../../core/auth/roles';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-add-user-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-user-modal.component.html',
  styleUrl: './add-user-modal.component.css'
})
export class AddUserModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly usersApi = inject(UsersApi);
  private readonly toast = inject(ToastService);

  @Output() closed = new EventEmitter<void>();
  @Output() created = new EventEmitter<void>();

  protected readonly submitting = signal(false);

  protected readonly roleOptions = computed(() =>
    ALL_ROLES
  );

  protected readonly roleLabels = ROLE_LABELS;

  protected readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    firstName: [''],
    lastName: [''],
    temporaryPassword: ['', [Validators.required, Validators.minLength(6)]],
    role: ['' as Role | '' , [Validators.required]]
  });

  protected onCancel(): void {
    if (this.submitting()) return;
    this.closed.emit();
  }

  protected onSubmit(): void {
    if (this.submitting()) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const dto: CreateUserRequestDto = {
      email: value.email,
      firstName: value.firstName?.trim() || null,
      lastName: value.lastName?.trim() || null,
      temporaryPassword: value.temporaryPassword,
      roles: [value.role as Role]
    };

    this.submitting.set(true);
    this.usersApi.create(dto).subscribe({
      next: () => {
        this.toast.success('User created');
        this.created.emit();
        this.closed.emit();
      },
      error: () => {
        this.toast.error('Failed to create user');
        this.submitting.set(false);
      }
    });
  }
}


