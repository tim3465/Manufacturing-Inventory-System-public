import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from './auth.service';

/**
 * Attaches `Authorization: Bearer <token>` to API calls when a token exists.
 * Dependency-free and standalone-friendly.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

  // Only attach to backend API calls; skip auth login itself.
  const isApiCall = req.url.includes('/api/');
  const isLoginCall = req.url.includes('/api/auth/login');

  if (!token || !isApiCall || isLoginCall) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    })
  );
};


