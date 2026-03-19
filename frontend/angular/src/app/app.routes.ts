import { Routes } from '@angular/router';
import { AppShellLayoutComponent } from './core/layout/app-shell-layout/app-shell-layout.component';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';
import { Roles } from './core/auth/roles';
import { AuthLoginPageComponent } from './features/auth/login.page';
import { DashboardPageComponent } from './features/dashboard/dashboard.page';
import { ShiftsPageComponent } from './features/machinist/shifts/shifts.page';
import { MachinistMachinesPageComponent } from './features/machinist/machines.page';
import { MyJobsPageComponent } from './features/machinist/my-jobs.page';
import { InventoryPageComponent } from './features/shipping/inventory/inventory.page';
import { ReceiveMaterialPageComponent } from './features/shipping/receive-material/receive-material.page';
import { ProductionPageComponent } from './features/supervisor/production/production.page';
import { CustomersPageComponent } from './features/supervisor/customers/customers.page';
import { NewOrderPageComponent } from './features/supervisor/new-order/new-order.page';
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
        path: 'machinist',
        canMatch: [roleGuard],
        data: { roles: [Roles.Machinist, Roles.Admin] },
        children: [
          { path: 'my-jobs', component: MyJobsPageComponent },
          { path: 'shifts', component: ShiftsPageComponent },
          { path: 'log-shift', redirectTo: 'shifts', pathMatch: 'full' },
          { path: 'machines', component: MachinistMachinesPageComponent }
        ]
      },
      {
        path: 'shipping',
        canMatch: [roleGuard],
        data: { roles: [Roles.Shipping, Roles.Admin] },
        children: [
          { path: 'receive-material', component: ReceiveMaterialPageComponent },
          { path: 'inventory', component: InventoryPageComponent }
        ]
      },
      {
        path: 'supervisor',
        canMatch: [roleGuard],
        data: { roles: [Roles.Supervisor, Roles.Admin] },
        children: [
          { path: 'production', component: ProductionPageComponent },
          { path: 'customers', component: CustomersPageComponent },
          { path: 'new-order', component: NewOrderPageComponent }
        ]
      },
      {
        path: 'admin',
        canMatch: [roleGuard],
        data: { roles: [Roles.Admin] },
        children: [
          { path: 'machines', component: MachinesPageComponent },
          { path: 'users', component: UsersPageComponent },
          { path: 'settings', component: SettingsPageComponent }
        ]
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
