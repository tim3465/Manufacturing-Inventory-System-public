import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-machines-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './machines.page.html',
  styleUrl: './machines.page.css'
})
export class MachinesPageComponent {
  protected readonly machines = [
    { name: 'HAAS VF-2', status: 'Online', utilization: '82%' },
    { name: 'Mazak QT-200', status: 'Maintenance', utilization: 'N/A' },
    { name: 'Okuma Genos', status: 'Online', utilization: '75%' }
  ];
}

