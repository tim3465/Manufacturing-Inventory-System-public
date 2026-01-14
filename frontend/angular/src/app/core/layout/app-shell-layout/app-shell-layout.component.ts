import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppHeaderComponent } from '../app-header/app-header.component';
import { AppSidebarComponent } from '../app-sidebar/app-sidebar.component';

@Component({
  selector: 'app-shell-layout',
  standalone: true,
  imports: [RouterOutlet, AppHeaderComponent, AppSidebarComponent],
  templateUrl: './app-shell-layout.component.html',
  styleUrl: './app-shell-layout.component.css',
  host: {
    class: 'block min-h-screen bg-[var(--bg)] text-[var(--fg)]'
  }
})
export class AppShellLayoutComponent {}

