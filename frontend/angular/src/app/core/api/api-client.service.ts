import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { finalize, shareReplay, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiCacheService } from './api-cache.service';

type QueryParams =
  | Record<string, string | number | boolean | null | undefined>
  | HttpParams
  | undefined;

@Injectable({ providedIn: 'root' })
export class ApiClient {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(
    private readonly http: HttpClient,
    private readonly cache: ApiCacheService
  ) {}

  get<T>(path: string, params?: QueryParams, headers?: HttpHeaders): Observable<T> {
    return this.http.get<T>(this.url(path), {
      params: this.toHttpParams(params),
      headers,
    });
  }

 getCached<T>(path: string, params?: QueryParams, headers?: HttpHeaders): Observable<T> {

  const now = Date.now();
  const key = this.getCacheKey(path,params)

  const cached =  this.cache.get<T>(key, now);

  if (cached !== undefined) {
    return of(cached);
  }

  const inflight = this.cache.getInflight<T>(key);
  if (inflight) return inflight;

  const ttlMs = 5 * 60 * 1000;

  const request$ = this.get<T>(path,params,headers)
    .pipe(
      tap((value) => this.cache.set(key, value, Date.now() + ttlMs)),
      shareReplay({ bufferSize: 1, refCount: false }),
      finalize(() => this.cache.clearInflight(key))
    );

  this.cache.setInflight(key, request$ as Observable<unknown>);
  return request$;
}

clearGetCache(path: string, params?: QueryParams): void {
  const key = this.getCacheKey(path,params)
  this.cache.deleteByKey(key);
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

  private getCacheKey(path: string, params?: QueryParams):string{
    const httpParams = this.toHttpParams(params);
    const requestUrl = this.url(path);
    const paramsString = httpParams?.toString() ?? '';
    const keyUrl = paramsString ? `${requestUrl}?${paramsString}` : requestUrl;


    const key = `GET ${keyUrl}`;

    return key;
  }
}
