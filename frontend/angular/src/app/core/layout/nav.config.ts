import { Role } from './role.model';
import { Roles } from '../auth/roles';

export interface NavLink {
  label: string;
  path: string;
  roles?: Role[];
}

export interface RoleNavGroup {
  title: Role;
  roles?: Role[];
  items: NavLink[];
}

export const TOP_NAV: NavLink[] = [
  {
    label: 'Dashboard',
    path: '/dashboard'
  }
];

export const ROLE_NAV_GROUPS: RoleNavGroup[] = [
  {
    title: Roles.Machinist,
    roles: [Roles.Machinist],
    items: [
      { label: 'My Jobs', path: '/machinist/my-jobs' },
      { label: 'Shifts', path: '/machinist/shifts' },
      { label: 'Machines', path: '/machinist/machines' }
    ]
  },
  {
    title: Roles.Shipping,
    roles: [Roles.Shipping],
    items: [
      { label: 'Receive Material', path: '/shipping/receive-material' },
      { label: 'Inventory', path: '/shipping/inventory' }
    ]
  },
  {
    title: Roles.Supervisor,
    roles: [Roles.Supervisor],
    items: [
      { label: 'Production', path: '/supervisor/production' },
      { label: 'Customers', path: '/supervisor/customers' }
    ]
  },
  {
    title: Roles.Admin,
    roles: [Roles.Admin],
    items: [
      { label: 'Machines', path: '/admin/machines' },
      { label: 'Users', path: '/admin/users' },
      { label: 'Settings', path: '/admin/settings' }
    ]
  }
];

