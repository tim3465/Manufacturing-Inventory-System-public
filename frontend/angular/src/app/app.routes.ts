import { Routes } from '@angular/router';
import { AppShellLayoutComponent } from './core/layout/app-shell-layout/app-shell-layout.component';
import { MockAuthLoginPageComponent } from './features/auth/login.page';
import { DashboardPageComponent } from './features/dashboard/dashboard.page';
import { LogShiftPageComponent } from './features/machinist/log-shift.page';
import { MyJobsPageComponent } from './features/machinist/my-jobs.page';
import { InventoryPageComponent } from './features/shipping/inventory.page';
import { ReceiveMaterialPageComponent } from './features/shipping/receive-material.page';
import { JobPlanningPageComponent } from './features/supervisor/job-planning.page';
import { OrdersPageComponent } from './features/supervisor/orders.page';
import { MachinesPageComponent } from './features/admin/machines.page';
import { UsersPageComponent } from './features/admin/users.page';
import { SettingsPageComponent } from './features/admin/settings.page';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: MockAuthLoginPageComponent
  },
  {
    path: '',
    component: AppShellLayoutComponent,
    children: [
      { path: 'dashboard', component: DashboardPageComponent },
      { path: 'machinist/my-jobs', component: MyJobsPageComponent },
      { path: 'machinist/log-shift', component: LogShiftPageComponent },
      { path: 'shipping/receive-material', component: ReceiveMaterialPageComponent },
      { path: 'shipping/inventory', component: InventoryPageComponent },
      { path: 'supervisor/orders', component: OrdersPageComponent },
      { path: 'supervisor/job-planning', component: JobPlanningPageComponent },
      { path: 'admin/machines', component: MachinesPageComponent },
      { path: 'admin/users', component: UsersPageComponent },
      { path: 'admin/settings', component: SettingsPageComponent }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
