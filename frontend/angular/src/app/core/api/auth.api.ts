// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/AuthController.cs
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginRequestDto } from '../dtos/auth/login-request.dto';
import { LoginResponseDto } from '../dtos/auth/login-response.dto';
import { ApiClient } from './api-client.service';

@Injectable({ providedIn: 'root' })
export class AuthApi {
  constructor(private readonly api: ApiClient) {}

  login(dto: LoginRequestDto): Observable<LoginResponseDto> {
    return this.api.post<LoginResponseDto>('/auth/login', dto);
  }

}


