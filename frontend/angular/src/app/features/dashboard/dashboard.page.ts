import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.page.html',
  styleUrl: './dashboard.page.css'
})
export class DashboardPageComponent {
  protected readonly stats = [
    { label: 'Open Jobs', value: '12' },
    { label: 'Machines Active', value: '7' },
    { label: 'Late Orders', value: '2' }
  ];
}

