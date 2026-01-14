import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ThemeName, ThemeService } from '../theme/theme.service';
import { ROLE_LABELS, Role } from './role.model';
import { RoleService } from './role.service';

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
  protected readonly roleService = inject(RoleService);

  protected readonly roles = this.roleService.roles;
  protected readonly roleLabels = ROLE_LABELS;
  protected readonly selectedRole = this.roleService.role;

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

  onRoleChange(roleValue: string): void {
    this.roleService.setRole(roleValue as Role);
  }

  private syncThemeFromDocument(): void {
    const body = document.body;
    const theme: ThemeName = body.classList.contains('theme-dark')
      ? 'theme-dark'
      : 'theme-light';
    this.currentTheme.set(theme);
  }
}

