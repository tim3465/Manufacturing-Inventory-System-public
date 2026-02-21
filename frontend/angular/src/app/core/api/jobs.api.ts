// Will contain the APIs found in: backend/CncApp/CncApp.Api/Controllers/JobsController.cs
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class JobsApi {
  constructor(private readonly http: HttpClient) {}
  // TODO: add endpoints as we implement features
}


