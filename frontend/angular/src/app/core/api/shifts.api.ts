// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/ShiftsController.cs
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ShiftsApi {
  constructor(private readonly http: HttpClient) {}
  // TODO: add endpoints as we implement features
}


