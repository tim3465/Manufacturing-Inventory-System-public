import { Role as AuthRole, Roles } from '../auth/roles';

export type Role = AuthRole;

export const ROLE_LABELS: Record<Role, string> = {
  [Roles.Admin]: 'Admin',
  [Roles.Machinist]: 'Machinist',
  [Roles.Shipping]: 'Shipping / Receiving',
  [Roles.Supervisor]: 'Supervisor',
  [Roles.User]: 'User'
};

export const ALL_ROLES: Role[] = [
  Roles.Admin,
  Roles.Machinist,
  Roles.Shipping,
  Roles.Supervisor,
  Roles.User
];

export const DEFAULT_ROLE: Role = Roles.Machinist;

