// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/UsersController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { CreateUserRequestDto, CreateUserResponseDto, UserDto } from '../dtos/users';
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
}


