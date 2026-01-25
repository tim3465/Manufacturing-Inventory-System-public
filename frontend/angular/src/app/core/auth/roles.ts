export const Roles = {
  Admin: 'Admin',
  Machinist: 'Machinist',
  Shipping: 'Shipping',
  Supervisor: 'Supervisor',
  User: 'User'
} as const;

export type Role = (typeof Roles)[keyof typeof Roles];

/** Single source of truth for role options and labels used across the frontend. */
export const ALL_ROLES: Role[] = [
  Roles.Admin,
  Roles.Machinist,
  Roles.Shipping,
  Roles.Supervisor,
  Roles.User
];

export const ROLE_LABELS: Record<Role, string> = {
  [Roles.Admin]: 'Admin',
  [Roles.Machinist]: 'Machinist',
  [Roles.Shipping]: 'Shipping / Receiving',
  [Roles.Supervisor]: 'Supervisor',
  [Roles.User]: 'User'
};

export const DEFAULT_ROLE: Role = Roles.Machinist;

export function roleLabel(role: Role): string {
  return ROLE_LABELS[role] ?? role;
}


