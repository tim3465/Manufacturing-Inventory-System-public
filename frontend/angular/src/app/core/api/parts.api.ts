// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/PartsController.cs
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class PartsApi {
  constructor(private readonly http: HttpClient) {}
  // TODO: add endpoints as we implement features
}


