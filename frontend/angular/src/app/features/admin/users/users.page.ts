import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { UsersApi } from '../../../core/api/users.api';
import { UserDto } from '../../../core/dtos/users';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { AuthService } from '../../../core/auth/auth.service';
import { AddUserModalComponent } from './add-user-modal/add-user-modal.component';

@Component({
  selector: 'app-users-page',
  standalone: true,
  imports: [CommonModule, AddUserModalComponent],
  templateUrl: './users.page.html',
  styleUrl: './users.page.css'
})
export class UsersPageComponent implements OnInit {
  private readonly usersApi = inject(UsersApi);
  private readonly toast = inject(ToastService);
  private readonly auth = inject(AuthService);

  protected readonly loading = signal<boolean>(true);
  protected readonly error = signal<string | null>(null);
  protected readonly users = signal<UserDto[]>([]);
  protected readonly showAll = signal<boolean>(false);
  protected readonly canShowAllToggle = computed(() => this.auth.isAdmin());
  protected readonly isAddUserOpen = signal<boolean>(false);

  protected readonly userRows = computed(() =>
    this.users().map((u) => ({
      id: u.id,
      name: this.displayName(u),
      userName: u.userName,
      status: u.inactivatedDateTime ? ('Inactive' as const) : ('Active' as const)
    }))
  );

  ngOnInit(): void {
    this.loadUsers();
  }

  protected openAddUser(): void {
    this.isAddUserOpen.set(true);
  }

  protected closeAddUser(): void {
    this.isAddUserOpen.set(false);
  }

  protected onToggleAllUsers(event: Event): void {
    const target = event.target as HTMLInputElement | null;
    this.showAll.set(!!target?.checked);
    this.loadUsers();
  }

  protected loadUsers(): void {
    this.loading.set(true);
    this.error.set(null);

    const request$ = this.showAll() ? this.usersApi.listAll() : this.usersApi.listActive();

    request$.subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: () => {
        const message = this.showAll()
          ? 'Failed to load all users'
          : 'Failed to load users';
        this.error.set(message);
        this.toast.error(message);
        this.loading.set(false);
      }
    });
  }

  private displayName(u: UserDto): string {
    const parts = [u.firstName, u.lastName].filter(
      (p): p is string => typeof p === 'string' && p.trim().length > 0
    );
    return parts.length ? parts.join(' ') : u.userName;
  }
}

