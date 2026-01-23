// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/UsersController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserDto } from '../dtos/users';
import { ApiClient } from './api-client.service';

@Injectable({ providedIn: 'root' })
export class UsersApi {
  constructor(private readonly api: ApiClient) {}

  /** GET /api/users - active users only */
  listActive(): Observable<UserDto[]> {
    return this.api.getCached<UserDto[]>('/users');
  }

  /** GET /api/users/all - all users including inactive (Admin-only) */
  listAll(): Observable<UserDto[]> {
    return this.api.getCached<UserDto[]>('/users/all');
  }
}


