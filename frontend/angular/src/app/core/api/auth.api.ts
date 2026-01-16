// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/AuthController.cs
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginRequestDto } from '../dtos/auth/login-request.dto';
import { LoginResponseDto } from '../dtos/auth/login-response.dto';

@Injectable({ providedIn: 'root' })
export class AuthApi {
  constructor(private readonly http: HttpClient) {}

  login(dto: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>('/api/auth/login', dto);
  }
}


