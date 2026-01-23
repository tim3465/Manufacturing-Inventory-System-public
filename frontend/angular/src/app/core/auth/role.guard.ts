import { inject } from '@angular/core';
import { CanMatchFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { Role } from './roles';

export const roleGuard: CanMatchFn = (route, segments) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isLoggedIn()) {
    const attemptedUrl = `/${segments.map((s) => s.path).join('/')}`;
    return router.createUrlTree(['/login'], {
      queryParams: { returnUrl: attemptedUrl }
    });
  }

  const requiredRoles = route.data?.['roles'] as Role[] | undefined;
  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  if (auth.hasAnyRole(requiredRoles)) {
    return true;
  }

  return router.createUrlTree(['/dashboard']);
};


