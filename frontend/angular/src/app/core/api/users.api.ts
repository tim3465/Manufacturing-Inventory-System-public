// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/UsersController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import {
  CreateUserRequestDto,
  CreateUserResponseDto,
  UpdateUserRolesRequestDto,
  UserDto,
  UserRolesDto
} from '../dtos/users';
import { ApiClient } from './api-client.service';

const _PATH = '/users';

@Injectable({ providedIn: 'root' })
export class UsersApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/users - active users only */
  listActive(): Observable<UserDto[]> {
    return this.api.getCached<UserDto[]>(`${_PATH}`);
  }

  /** GET /api/users/all - all users including inactive (Admin-only) */
  listAll(): Observable<UserDto[]> {
    return this.api.getCached<UserDto[]>(`${_PATH}/all`);
  }

  /** POST /api/users - create user (Admin-only) */
  create(dto: CreateUserRequestDto): Observable<CreateUserResponseDto> {
    return this.api.post<CreateUserResponseDto>(`${_PATH}`, dto).pipe(
      tap(() => {
        this.api.clearGetCache(`${_PATH}`);
        this.api.clearGetCache(`${_PATH}/all`);
      })
    );
  }

  /** GET /api/users/{id}/roles - get user's assigned roles (Admin-only) */
  getRoles(id: number): Observable<UserRolesDto> {
    return this.api.getCached<UserRolesDto>(`${_PATH}/${id}/roles`);
  }

  /** PATCH /api/users/{id} - replace user's roles (Admin-only) */
  updateRoles(id: number, dto: UpdateUserRolesRequestDto): Observable<boolean> {
    return this.api.patch<boolean>(`${_PATH}/${id}`, dto).pipe(
      tap(()=>{
        this.api.clearGetCache(`${_PATH}/${id}/roles`);
      })
    );
  }

  /** PATCH /api/users/{id}/inactivate - inactivate user (Admin-only) */
  inactivate(id: number): Observable<boolean> {
    return this.api.patch<boolean>(`${_PATH}/${id}/inactivate`, {}).pipe(
      tap(() => {
        this.api.clearGetCache(`${_PATH}`);
        this.api.clearGetCache(`${_PATH}/all`);
      })
    );
  }
}


