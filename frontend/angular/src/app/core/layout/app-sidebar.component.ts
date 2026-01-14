import { CommonModule } from '@angular/common';
import { Component, computed, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ROLE_LABELS, Role } from './role.model';
import { ROLE_NAV_GROUPS, TOP_NAV } from './nav.config';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './app-sidebar.component.html',
  styleUrl: './app-sidebar.component.css',
  host: {
    class: 'block h-full'
  }
})
export class AppSidebarComponent {
  protected readonly topLinks = TOP_NAV;
  protected readonly roleLabels = ROLE_LABELS;
  protected readonly groups = ROLE_NAV_GROUPS;

  private readonly expandedRole = signal<Role | null>(null);

  protected isExpanded = computed(
    () => (role: Role) => this.expandedRole() === role
  );

  protected toggle(role: Role): void {
    this.expandedRole.set(this.expandedRole() === role ? null : role);
  }
}

