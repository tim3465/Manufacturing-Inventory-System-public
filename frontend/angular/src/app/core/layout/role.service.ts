import { Injectable, signal } from '@angular/core';
import { ALL_ROLES, DEFAULT_ROLE, Role } from './role.model';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private readonly storageKey = 'cncapp.role';
  private readonly selectedRole = signal<Role>(
    this.readStoredRole() ?? DEFAULT_ROLE
  );

  readonly role = this.selectedRole.asReadonly();
  readonly roles = ALL_ROLES;

  setRole(role: Role): void {
    if (!this.roles.includes(role)) return;
    this.selectedRole.set(role);
    this.persistRole(role);
  }

  private readStoredRole(): Role | null {
    try {
      const value = localStorage.getItem(this.storageKey) as Role | null;
      return this.roles.includes(value as Role) ? (value as Role) : null;
    } catch {
      return null;
    }
  }

  private persistRole(role: Role): void {
    try {
      localStorage.setItem(this.storageKey, role);
    } catch {
      // best-effort persistence
    }
  }
}

