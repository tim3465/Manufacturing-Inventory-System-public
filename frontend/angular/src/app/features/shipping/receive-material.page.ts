import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-receive-material-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './receive-material.page.html',
  styleUrl: './receive-material.page.css'
})
export class ReceiveMaterialPageComponent {
  protected readonly receipts = [
    { id: 'PO-5521', supplier: 'Cascade Metals', status: 'Inspect' },
    { id: 'PO-5520', supplier: 'North Ridge', status: 'Staged' },
    { id: 'PO-5518', supplier: 'Atlas Steel', status: 'Received' }
  ];
}

