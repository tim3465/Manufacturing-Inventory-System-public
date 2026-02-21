import { inject } from '@angular/core';
import { CanMatchFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanMatchFn = (_route, segments) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn()) {
    return true;
  }

  const attemptedUrl = `/${segments.map((s) => s.path).join('/')}`;
  return router.createUrlTree(['/login'], {
    queryParams: { returnUrl: attemptedUrl }
  });
};


