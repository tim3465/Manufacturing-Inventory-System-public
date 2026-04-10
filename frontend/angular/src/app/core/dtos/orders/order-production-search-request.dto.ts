export interface OrderProductionSearchRequestDto {
  customerName?: string;
  partName?: string;
  partNumber?: string;
  sortColumn: string;
  sortDirection: string;
  page: number;
  pageSize: number;
}
