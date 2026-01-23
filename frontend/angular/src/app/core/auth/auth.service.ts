import { Injectable, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthApi } from '../api';
import { HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { ToastService } from '../ui/toast/toast.service';
import { Role, Roles } from './roles';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly router = inject(Router);
  private readonly storageKey = 'cncapp.accessToken';
  private readonly authApi = inject(AuthApi);
  private readonly toast = inject(ToastService);
  private cachedRoles: Role[] | null = null;
  private cachedDisplayName: string | null = null;



login(email: string, password: string, returnUrl?: string | null): Observable<string | null> {
  const target = returnUrl && returnUrl.startsWith('/') ? returnUrl : '/dashboard';
   return this.authApi.login({ email, password }).pipe(
    tap((res) => {
      this.setToken(res.accessToken);
      this.toast.success('Logged in');
      void this.router.navigateByUrl(target);
    }),
    map(() => null), // no error
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse && err.status === 401) {
        const message = 'Invalid email or password';
        this.toast.error(message);
        return of(message);
      }
      const message = 'Login failed';
      this.toast.error(message);
      return of(message);
    })
  );
}


  getToken(): string | null {
    return localStorage.getItem(this.storageKey);
  }

  setToken(token: string): void {
    localStorage.setItem(this.storageKey, token);
    this.cachedRoles = this.parseRolesFromToken(token);
    this.cachedDisplayName = this.parseDisplayNameFromToken(token);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getRoles(): Role[] {
    if (this.cachedRoles) return this.cachedRoles;

    const token = this.getToken();
    this.cachedRoles = token ? this.parseRolesFromToken(token) : [];
    return this.cachedRoles;
  }

  getDisplayName(): string | null {
    if (this.cachedDisplayName) return this.cachedDisplayName;
    const token = this.getToken();
    this.cachedDisplayName = token ? this.parseDisplayNameFromToken(token) : null;
    return this.cachedDisplayName;
  }


  isAdmin(): boolean {
    return this.getRoles().includes(Roles.Admin);
  }

  hasAnyRole(roles: Role[]): boolean {
    if (this.isAdmin()) return true;
    const current = this.getRoles();
    return roles.some((role) => current.includes(role));
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.clearCache();
    this.toast.info('Logged out');
    void this.router.navigateByUrl('/login');
  }

  clearCache(): void {
    this.cachedRoles = null;
    this.cachedDisplayName = null;
  }


  private parseRolesFromToken(token: string): Role[] {
    const payload = this.decodeJwtPayload(token);
    if (!payload || typeof payload !== 'object') return [];

    const rawRoles =
      payload['role'] ??
      payload['roles'] ??
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (!rawRoles) return [];

    const roles = Array.isArray(rawRoles) ? rawRoles : [rawRoles];
    return roles
      .filter((role): role is string => typeof role === 'string')
      .map((role) => role.trim())
      .filter((role) => role.length > 0) as Role[];
  }

  private parseDisplayNameFromToken(token: string): string | null {
    const payload = this.decodeJwtPayload(token);
    if (!payload || typeof payload !== 'object') return null;

    const rawName =
      payload['name'] ??
      payload['unique_name'] ??
      payload['preferred_username'] ??
      payload['email'] ??
      payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
      payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];

    if (typeof rawName !== 'string') return null;
    const trimmed = rawName.trim();
    return trimmed.length > 0 ? trimmed : null;
  }


  private decodeJwtPayload(token: string): Record<string, unknown> | null {
    const parts = token.split('.');
    if (parts.length < 2) return null;

    const payload = parts[1];
    try {
      const decoded = this.decodeBase64Url(payload);
      return JSON.parse(decoded) as Record<string, unknown>;
    } catch {
      return null;
    }
  }

  private decodeBase64Url(value: string): string {
    const normalized = value.replace(/-/g, '+').replace(/_/g, '/');
    const padded = normalized.padEnd(Math.ceil(normalized.length / 4) * 4, '=');
    return atob(padded);
  }
}

