export interface CreateUserRequestDto {
  email: string;
  firstName?: string | null;
  lastName?: string | null;
  temporaryPassword: string;
  roles: string[];
}


