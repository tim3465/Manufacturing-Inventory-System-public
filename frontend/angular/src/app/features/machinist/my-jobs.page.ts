import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-my-jobs-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-jobs.page.html',
  styleUrl: './my-jobs.page.css'
})
export class MyJobsPageComponent {
  protected readonly jobs = [
    { id: 'J-1021', part: 'Housing Block', status: 'In Progress' },
    { id: 'J-1022', part: 'Spindle Bracket', status: 'Queued' },
    { id: 'J-1019', part: 'Valve Plate', status: 'Needs QA' }
  ];
}

