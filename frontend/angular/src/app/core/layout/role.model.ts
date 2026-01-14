export type Role = 'Machinist' | 'ShippingReceiving' | 'Supervisor' | 'Admin';

export const ROLE_LABELS: Record<Role, string> = {
  Machinist: 'Machinist',
  ShippingReceiving: 'Shipping / Receiving',
  Supervisor: 'Supervisor',
  Admin: 'Admin'
};

export const ALL_ROLES: Role[] = [
  'Machinist',
  'ShippingReceiving',
  'Supervisor',
  'Admin'
];

export const DEFAULT_ROLE: Role = 'Machinist';

