import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ThemeName, ThemeService } from '../theme/theme.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app-header.component.html',
  styleUrl: './app-header.component.css',
  host: { class: 'block w-full' },
})
export class AppHeaderComponent implements OnInit {
  private readonly themeService = inject(ThemeService);

  protected readonly currentTheme = signal<ThemeName>('theme-light');

  ngOnInit(): void {
    this.syncThemeFromDocument();
  }

  toggleTheme(): void {
    const next: ThemeName =
      this.currentTheme() === 'theme-dark' ? 'theme-light' : 'theme-dark';

    this.themeService.setTheme(next); // <-- public wrapper you added
    this.currentTheme.set(next);
  }

  private syncThemeFromDocument(): void {
    const body = document.body;
    const theme: ThemeName = body.classList.contains('theme-dark')
      ? 'theme-dark'
      : 'theme-light';
    this.currentTheme.set(theme);
  }
}

