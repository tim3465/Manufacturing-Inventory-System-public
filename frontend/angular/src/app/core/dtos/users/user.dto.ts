export interface UserDto {
  id: number;
  userName: string;
  firstName: string | null;
  lastName: string | null;
  inactivatedDateTime: string | null; // ISO string from backend DateTimeOffset?
}


