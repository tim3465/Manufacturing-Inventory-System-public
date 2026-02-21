import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ROLE_LABELS, Role } from '../role.model';
import { ROLE_NAV_GROUPS, TOP_NAV } from '../nav.config';
import { AuthService } from '../../auth/auth.service';

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
  private readonly auth = inject(AuthService);
  protected readonly topLinks = TOP_NAV;
  protected readonly roleLabels = ROLE_LABELS;
  protected readonly groups = ROLE_NAV_GROUPS;
  @Input() mobileNavOpen = false;
  @Output() closeMenu = new EventEmitter<void>();

  protected readonly visibleTopLinks = computed(() =>
    this.topLinks.filter((link) => this.canView(link.roles))
  );

  protected readonly visibleGroups = computed(() =>
    this.groups
      .map((group) => ({
        ...group,
        items: group.items.filter((item) => this.canView(item.roles))
      }))
      .filter((group) => this.canView(group.roles) && group.items.length > 0)
  );

  private readonly expandedRole = signal<Role | null>(null);

  protected isExpanded = computed(
    () => (role: Role) => this.expandedRole() === role
  );

  protected toggle(role: Role): void {
    this.expandedRole.set(this.expandedRole() === role ? null : role);
  }

  protected onClose(): void {
    this.closeMenu.emit();
  }

  private canView(roles?: Role[]): boolean {
    if (!roles || roles.length === 0) return true;
    return this.auth.hasAnyRole(roles);
  }
}

