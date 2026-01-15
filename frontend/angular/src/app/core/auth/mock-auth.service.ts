import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MockAuthService {
  private readonly storageKey = 'cncapp.mockAuth';

  isLoggedIn(): boolean {
    return localStorage.getItem(this.storageKey) === 'true';
  }

  login(username: string, password: string): void {
    if (!username || !password) {
      return;
    }
    localStorage.setItem(this.storageKey, 'true');
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
  }
}

