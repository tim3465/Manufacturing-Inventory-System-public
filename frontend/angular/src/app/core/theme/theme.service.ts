import { Injectable } from '@angular/core';

export type ThemeName = 'theme-light' | 'theme-dark' ;

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly storageKey = 'cncapp.theme';
  private readonly themes: ThemeName[] = ['theme-light', 'theme-dark'];

  initialize(): void {
    const theme = this.resolvePreferredTheme();
    this.applyTheme(theme);
  }

  private resolvePreferredTheme(): ThemeName {
    const stored = this.readStoredTheme();
    if (stored) return stored;

    if (window.matchMedia?.('(prefers-color-scheme: dark)').matches) {
      return 'theme-dark';
    }

    return 'theme-light';
  }

  private applyTheme(theme: ThemeName): void {
    const body = document.body;
    body.classList.remove(...this.themes);
    body.classList.add(theme);
    this.persistTheme(theme);
  }

  private readStoredTheme(): ThemeName | null {
    try {
      const value = localStorage.getItem(this.storageKey);
      return value === 'theme-light' || value === 'theme-dark'  ? value : null;
    } catch {
      return null;
    }
  }

  private persistTheme(theme: ThemeName): void {
    try {
      localStorage.setItem(this.storageKey, theme);
    } catch {
      // best-effort
    }
  }
}
