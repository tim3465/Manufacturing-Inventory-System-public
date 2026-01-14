import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NAV_SECTIONS } from './nav.config';
import { RoleService } from './role.service';

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
  private readonly roleService = inject(RoleService);
  protected readonly role = this.roleService.role;

  protected readonly sections = computed(() =>
    NAV_SECTIONS.map((section) => ({
      title: section.title,
      items: section.items.filter((item) => item.roles.includes(this.role()))
    })).filter((section) => section.items.length > 0)
  );
}

