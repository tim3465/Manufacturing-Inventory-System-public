import { Role } from './role.model';

export interface NavLink {
  label: string;
  path: string;
}

export interface RoleNavGroup {
  role: Role;
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
    role: 'Machinist',
    items: [
      { label: 'My Jobs', path: '/machinist/my-jobs' },
      { label: 'Log Shift', path: '/machinist/log-shift' }
    ]
  },
  {
    role: 'ShippingReceiving',
    items: [
      { label: 'Receive Material', path: '/shipping/receive-material' },
      { label: 'Inventory', path: '/shipping/inventory' }
    ]
  },
  {
    role: 'Supervisor',
    items: [
      { label: 'Orders', path: '/supervisor/orders' },
      { label: 'Job Planning', path: '/supervisor/job-planning' }
    ]
  },
  {
    role: 'Admin',
    items: [
      { label: 'Machines', path: '/admin/machines' },
      { label: 'Users', path: '/admin/users' },
      { label: 'Settings', path: '/admin/settings' }
    ]
  }
];

