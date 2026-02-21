import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-log-shift-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './log-shift.page.html',
  styleUrl: './log-shift.page.css'
})
export class LogShiftPageComponent {
  protected readonly tasks = [
    { label: 'Setup time', value: '00:35' },
    { label: 'Run time', value: '02:10' },
    { label: 'Downtime', value: '00:12' }
  ];
}

