import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './settings.page.html',
  styleUrl: './settings.page.css'
})
export class SettingsPageComponent {
  protected readonly settings = [
    { label: 'Shop timezone', value: 'UTC-05:00' },
    { label: 'Default theme', value: 'Auto' },
    { label: 'Maintenance window', value: 'Sundays 04:00' }
  ];
}

