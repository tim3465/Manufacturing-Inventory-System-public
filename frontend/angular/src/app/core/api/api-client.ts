import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

type QueryParams =
  | Record<string, string | number | boolean | null | undefined>
  | HttpParams
  | undefined;

@Injectable({ providedIn: 'root' })
export class ApiClient {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private readonly http: HttpClient) {}

  get<T>(path: string, params?: QueryParams, headers?: HttpHeaders): Observable<T> {
    return this.http.get<T>(this.url(path), {
      params: this.toHttpParams(params),
      headers,
    });
  }

  post<T>(path: string, body: unknown, headers?: HttpHeaders): Observable<T> {
    return this.http.post<T>(this.url(path), body, { headers });
  }

  put<T>(path: string, body: unknown, headers?: HttpHeaders): Observable<T> {
    return this.http.put<T>(this.url(path), body, { headers });
  }

  delete<T>(path: string, params?: QueryParams, headers?: HttpHeaders): Observable<T> {
    return this.http.delete<T>(this.url(path), {
      params: this.toHttpParams(params),
      headers,
    });
  }

  private url(path: string): string {
    const cleanBase = this.baseUrl.replace(/\/$/, '');
    const cleanPath = path.startsWith('/') ? path : `/${path}`;
    return `${cleanBase}${cleanPath}`;
  }

  private toHttpParams(params?: QueryParams): HttpParams | undefined {
    if (!params) return undefined;
    if (params instanceof HttpParams) return params;

    let httpParams = new HttpParams();
    for (const [key, value] of Object.entries(params)) {
      if (value === null || value === undefined) continue;
      httpParams = httpParams.set(key, String(value));
    }
    return httpParams;
  }
}
