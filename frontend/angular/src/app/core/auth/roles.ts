export const Roles = {
  Admin: 'Admin',
  Machinist: 'Machinist',
  Shipping: 'Shipping',
  Supervisor: 'Supervisor',
  User: 'User'
} as const;

export type Role = (typeof Roles)[keyof typeof Roles];


