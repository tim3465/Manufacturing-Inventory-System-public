import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

type CacheEntry = {
  expiresAtMs: number;
  value: unknown;
};

@Injectable({ providedIn: 'root' })
export class ApiCacheService {
  private readonly cache = new Map<string, CacheEntry>();
  private readonly inflight = new Map<string, Observable<unknown>>();

  get<T>(key: string, nowMs: number): T | undefined {
    const entry = this.cache.get(key);
    if (!entry) return undefined;
    if (entry.expiresAtMs <= nowMs) {
      this.cache.delete(key);
      return undefined;
    }
    return entry.value as T;
  }

  set(key: string, value: unknown, expiresAtMs: number): void {
    this.cache.set(key, { value, expiresAtMs });
  }

deleteByKey(key: string): void {
  this.cache.delete(key);
  this.inflight.delete(key);
}


  getInflight<T>(key: string): Observable<T> | undefined {
    return this.inflight.get(key) as Observable<T> | undefined;
  }

  setInflight(key: string, obs$: Observable<unknown>): void {
    this.inflight.set(key, obs$);
  }

  clearInflight(key: string): void {
    this.inflight.delete(key);
  }

  clear(): void {
    this.cache.clear();
    this.inflight.clear();
  }
}


