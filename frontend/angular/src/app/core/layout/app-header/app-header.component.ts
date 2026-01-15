import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MockAuthService } from '../../auth/mock-auth.service';
import { ThemeName, ThemeService } from '../../theme/theme.service';

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
  private readonly router = inject(Router);
  private readonly mockAuth = inject(MockAuthService);

  protected readonly currentTheme = signal<ThemeName>('theme-light');
  @Input() mobileNavOpen = false;
  @Output() menuToggle = new EventEmitter<void>();

  ngOnInit(): void {
    this.syncThemeFromDocument();
  }

  toggleTheme(): void {
    const next: ThemeName =
      this.currentTheme() === 'theme-dark' ? 'theme-light' : 'theme-dark';

    this.themeService.setTheme(next); // <-- public wrapper you added
    this.currentTheme.set(next);
  }

  onToggleMenu(): void {
    this.menuToggle.emit();
  }

  logout(): void {
    this.mockAuth.logout();
    void this.router.navigateByUrl('/login');
  }

  private syncThemeFromDocument(): void {
    const body = document.body;
    const theme: ThemeName = body.classList.contains('theme-dark')
      ? 'theme-dark'
      : 'theme-light';
    this.currentTheme.set(theme);
  }
}

