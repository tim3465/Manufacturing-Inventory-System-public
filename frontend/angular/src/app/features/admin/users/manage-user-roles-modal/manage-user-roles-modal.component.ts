import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UsersApi } from '../../../../core/api/users.api';
import { ALL_ROLES, ROLE_LABELS, Role } from '../../../../core/auth/roles';
import { UpdateUserRolesRequestDto } from '../../../../core/dtos/users';
import { ToastService } from '../../../../core/ui/toast/toast.service';

@Component({
  selector: 'app-manage-user-roles-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manage-user-roles-modal.component.html',
  styleUrl: './manage-user-roles-modal.component.css'
})
export class ManageUserRolesModalComponent implements OnInit {
  private readonly usersApi = inject(UsersApi);
  private readonly toast = inject(ToastService);
  private readonly roleSet = new Set<Role>(ALL_ROLES);

  @Input({ required: true }) userId!: number;
  @Input({ required: true }) fullName!: string;
  @Input({ required: true }) email!: string;

  @Output() closed = new EventEmitter<void>();
  @Output() updated = new EventEmitter<void>();

  protected readonly loading = signal<boolean>(true);
  protected readonly submitting = signal<boolean>(false);
  protected readonly selectedRoles = signal<Set<Role>>(new Set<Role>());
  protected readonly roleOptions = ALL_ROLES;
  protected readonly roleLabels = ROLE_LABELS;

  ngOnInit(): void {
    this.loadRoles();
  }

  protected onCancel(): void {
    if (this.submitting()) return;
    this.closed.emit();
  }

  protected onRoleToggle(role: Role, event: Event): void {
    const checked = (event.target as HTMLInputElement | null)?.checked === true;
    const next = new Set(this.selectedRoles());
    if (checked) {
      next.add(role);
    } else {
      next.delete(role);
    }
    this.selectedRoles.set(next);
  }

  protected isRoleSelected(role: Role): boolean {
    return this.selectedRoles().has(role);
  }

  protected onSubmit(): void {
    if (this.loading() || this.submitting()) return;

    const dto: UpdateUserRolesRequestDto = {
      roles: Array.from(this.selectedRoles())
    };

    this.submitting.set(true);
    this.usersApi.updateRoles(this.userId, dto).subscribe({
      next: () => {
        this.toast.success('User roles updated');
        this.updated.emit();
        this.closed.emit();
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to update user roles');
        this.submitting.set(false);
      }
    });
  }

  private loadRoles(): void {
    this.loading.set(true);
    this.usersApi.getRoles(this.userId).subscribe({
      next: (result) => {
        const roles = result.roles.filter((role): role is Role => this.roleSet.has(role as Role));
        this.selectedRoles.set(new Set<Role>(roles));
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.toast.errorMessage(err, undefined, 'Failed to load user roles');
        this.loading.set(false);
      }
    });
  }
}


