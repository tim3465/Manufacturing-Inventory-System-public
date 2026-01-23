import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
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
  private readonly auth = inject(AuthService);

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
    this.auth.logout();
  }

  protected displayName(): string {
    return this.auth.getDisplayName() ?? 'User';
  }

  protected displayInitial(): string {
    const name = this.displayName().trim();
    return name ? name[0].toUpperCase() : 'U';
  }

  private syncThemeFromDocument(): void {
    const body = document.body;
    const theme: ThemeName = body.classList.contains('theme-dark')
      ? 'theme-dark'
      : 'theme-light';
    this.currentTheme.set(theme);
  }
}

