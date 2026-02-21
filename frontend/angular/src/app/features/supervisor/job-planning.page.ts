import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-job-planning-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './job-planning.page.html',
  styleUrl: './job-planning.page.css'
})
export class JobPlanningPageComponent {
  protected readonly steps = [
    { title: 'Review requirements', owner: 'Planner', status: 'Ready' },
    { title: 'Schedule machine time', owner: 'Scheduler', status: 'In Progress' },
    { title: 'Publish traveler', owner: 'Planner', status: 'Pending' }
  ];
}

