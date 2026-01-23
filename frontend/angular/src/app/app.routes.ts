import { Routes } from '@angular/router';
import { AppShellLayoutComponent } from './core/layout/app-shell-layout/app-shell-layout.component';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';
import { Roles } from './core/auth/roles';
import { AuthLoginPageComponent } from './features/auth/login.page';
import { DashboardPageComponent } from './features/dashboard/dashboard.page';
import { LogShiftPageComponent } from './features/machinist/log-shift.page';
import { MyJobsPageComponent } from './features/machinist/my-jobs.page';
import { InventoryPageComponent } from './features/shipping/inventory.page';
import { ReceiveMaterialPageComponent } from './features/shipping/receive-material.page';
import { JobPlanningPageComponent } from './features/supervisor/job-planning.page';
import { OrdersPageComponent } from './features/supervisor/orders.page';
import { MachinesPageComponent } from './features/admin/machines/machines.page';
import { UsersPageComponent } from './features/admin/users/users.page';
import { SettingsPageComponent } from './features/admin/settings/settings.page';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: AuthLoginPageComponent
  },
  {
    path: '',
    component: AppShellLayoutComponent,
    canMatch: [authGuard],
    children: [
      { path: 'dashboard', component: DashboardPageComponent },
      {
        path: '',
        canMatch: [roleGuard],
        data: { roles: [Roles.Machinist, Roles.Admin] },
        children: [
          { path: 'machinist/my-jobs', component: MyJobsPageComponent },
          { path: 'machinist/log-shift', component: LogShiftPageComponent }
        ]
      },
      {
        path: '',
        canMatch: [roleGuard],
        data: { roles: [Roles.Shipping, Roles.Admin] },
        children: [
          { path: 'shipping/receive-material', component: ReceiveMaterialPageComponent },
          { path: 'shipping/inventory', component: InventoryPageComponent }
        ]
      },
      {
        path: '',
        canMatch: [roleGuard],
        data: { roles: [Roles.Supervisor, Roles.Admin] },
        children: [
          { path: 'supervisor/orders', component: OrdersPageComponent },
          { path: 'supervisor/job-planning', component: JobPlanningPageComponent }
        ]
      },
      {
        path: '',
        canMatch: [roleGuard],
        data: { roles: [Roles.Admin] },
        children: [
          { path: 'admin/machines', component: MachinesPageComponent },
          { path: 'admin/users', component: UsersPageComponent },
          { path: 'admin/settings', component: SettingsPageComponent }
        ]
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
