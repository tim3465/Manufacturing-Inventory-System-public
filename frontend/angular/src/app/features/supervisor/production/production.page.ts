import { Component, ViewChild, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AddPartModalComponent } from './add-part-modal/add-part-modal.component';
import { AssignStockLotModalComponent } from './assign-stock-lot-modal/assign-stock-lot-modal.component';
import { ProductionJobsTabComponent, JobProductionRow } from './tabs/production-jobs-tab/production-jobs-tab.component';
import { ProductionOrdersTabComponent } from './tabs/production-orders-tab/production-orders-tab.component';
import { ProductionPartsTabComponent } from './tabs/production-parts-tab/production-parts-tab.component';
import { ProductionShiftsTabComponent } from './tabs/production-shifts-tab/production-shifts-tab.component';

type Tab = 'orders' | 'jobs' | 'parts' | 'shifts';

interface TabDef {
  id: Tab;
  label: string;
}

@Component({
  selector: 'app-production-page',
  standalone: true,
  imports: [AddPartModalComponent, AssignStockLotModalComponent, ProductionJobsTabComponent, ProductionOrdersTabComponent, ProductionPartsTabComponent, ProductionShiftsTabComponent],
  templateUrl: './production.page.html',
  styleUrl: './production.page.css'
})
export class ProductionPageComponent {
  private readonly router = inject(Router);

  protected readonly selectedTab = signal<Tab>('orders');

  protected readonly tabs: TabDef[] = [
    { id: 'orders', label: 'Orders' },
    { id: 'jobs',   label: 'Jobs' },
    { id: 'parts',  label: 'Parts' },
    { id: 'shifts', label: 'Shifts' }
  ];

  protected readonly isAddPartOpen = signal(false);

  @ViewChild(ProductionOrdersTabComponent) ordersTab?: ProductionOrdersTabComponent;
  @ViewChild(ProductionPartsTabComponent) partsTab?: ProductionPartsTabComponent;
  @ViewChild(ProductionShiftsTabComponent) shiftsTab?: ProductionShiftsTabComponent;
  @ViewChild(ProductionJobsTabComponent) jobsTab?: ProductionJobsTabComponent;

  protected readonly selectedJobForLot = signal<JobProductionRow | null>(null);

  protected selectTab(tab: Tab): void {
    this.selectedTab.set(tab);
  }

  protected openAddPart(): void {
    this.isAddPartOpen.set(true);
  }

  protected onPartCreated(): void {
    this.partsTab?.refresh();
  }

  protected openAssignLotModal(job: JobProductionRow): void {
    this.selectedJobForLot.set(job);
  }

  protected onLotAssigned(): void {
    this.selectedJobForLot.set(null);
    this.jobsTab?.refresh();
  }

  protected goToNewOrder(): void {
    this.router.navigate(['/supervisor/new-order']);
  }
}
