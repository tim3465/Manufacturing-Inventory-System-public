import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-users-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './users.page.html',
  styleUrl: './users.page.css'
})
export class UsersPageComponent {
  protected readonly users = [
    { name: 'Amy Chen', role: 'Supervisor', status: 'Active' },
    { name: 'Dev Patel', role: 'Machinist', status: 'Active' },
    { name: 'Morgan Lee', role: 'Admin', status: 'Invited' }
  ];
}

