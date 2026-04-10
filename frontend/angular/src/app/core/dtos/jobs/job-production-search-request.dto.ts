export interface JobProductionSearchRequestDto {
  dueDateFrom?: string;
  dueDateTo?: string;
  orderNumber?: string;
  partName?: string;
  partNumber?: string;
  machineName?: string;
  lotNumber?: string;
  sortColumn: string;
  sortDirection: string;
  page: number;
  pageSize: number;
}
