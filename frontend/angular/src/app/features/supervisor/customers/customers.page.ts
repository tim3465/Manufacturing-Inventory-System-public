import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { CustomersApi } from '../../../core/api/customers.api';
import { CustomerDto } from '../../../core/dtos/customers/customer.dto';
import { ToastService } from '../../../core/ui/toast/toast.service';
import { AddCustomerModalComponent } from './add-customer-modal/add-customer-modal.component';

@Component({
  selector: 'app-customers-page',
  standalone: true,
  imports: [CommonModule, AddCustomerModalComponent],
  templateUrl: './customers.page.html',
  styleUrl: './customers.page.css'
})
export class CustomersPageComponent implements OnInit {
  private readonly customersApi = inject(CustomersApi);
  private readonly toast = inject(ToastService);

  protected readonly loading = signal<boolean>(true);
  protected readonly error = signal<string | null>(null);
  protected readonly customers = signal<CustomerDto[]>([]);
  protected readonly isAddCustomerOpen = signal<boolean>(false);

  ngOnInit(): void {
    this.loadCustomers();
  }

  protected openAddCustomer(): void {
    this.isAddCustomerOpen.set(true);
  }

  protected closeAddCustomer(): void {
    this.isAddCustomerOpen.set(false);
  }

  protected loadCustomers(): void {
    this.loading.set(true);
    this.error.set(null);

    this.customersApi.listActive().subscribe({
      next: (customers) => {
        this.customers.set(customers);
        this.loading.set(false);
      },
      error: () => {
        const message = 'Failed to load customers';
        this.error.set(message);
        this.toast.error(message);
        this.loading.set(false);
      }
    });
  }
}
