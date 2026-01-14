import { ALL_ROLES, Role } from './role.model';

export interface NavItem {
  label: string;
  path: string;
  roles: Role[];
}

export interface NavSection {
  title: string;
  items: NavItem[];
}

export const NAV_SECTIONS: NavSection[] = [
  {
    title: 'Overview',
    items: [
      {
        label: 'Dashboard',
        path: '/dashboard',
        roles: ALL_ROLES
      }
    ]
  },
  {
    title: 'Machinist',
    items: [
      {
        label: 'My Jobs',
        path: '/machinist/my-jobs',
        roles: ['Machinist']
      },
      {
        label: 'Log Shift',
        path: '/machinist/log-shift',
        roles: ['Machinist']
      }
    ]
  },
  {
    title: 'Shipping / Receiving',
    items: [
      {
        label: 'Receive Material',
        path: '/shipping/receive-material',
        roles: ['ShippingReceiving']
      },
      {
        label: 'Inventory',
        path: '/shipping/inventory',
        roles: ['ShippingReceiving']
      }
    ]
  },
  {
    title: 'Supervisor',
    items: [
      {
        label: 'Orders',
        path: '/supervisor/orders',
        roles: ['Supervisor']
      },
      {
        label: 'Job Planning',
        path: '/supervisor/job-planning',
        roles: ['Supervisor']
      }
    ]
  },
  {
    title: 'Admin',
    items: [
      {
        label: 'Machines',
        path: '/admin/machines',
        roles: ['Admin']
      },
      {
        label: 'Users',
        path: '/admin/users',
        roles: ['Admin']
      },
      {
        label: 'Settings',
        path: '/admin/settings',
        roles: ['Admin']
      }
    ]
  }
];

