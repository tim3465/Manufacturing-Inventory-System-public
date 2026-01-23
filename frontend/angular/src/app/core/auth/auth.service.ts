import { Injectable, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthApi } from '../api';
import { HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { ToastService } from '../ui/toast/toast.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly router = inject(Router);
  private readonly storageKey = 'cncapp.accessToken';
  private readonly authApi = inject(AuthApi);
  private readonly route = inject(ActivatedRoute);
  private readonly toast = inject(ToastService);



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
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.toast.info('Logged out');
    void this.router.navigateByUrl('/login');
  }
}

