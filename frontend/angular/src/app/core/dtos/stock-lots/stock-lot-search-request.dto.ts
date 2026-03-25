export interface StockLotSearchRequestDto {
  lotNumber?: string;
  checkedInFrom?: string;
  checkedInTo?: string;
  diameterExact?: number;
  diameterMin?: number;
  diameterMax?: number;
  sortColumn?: string;
  sortDirection?: string;
  page?: number;
  pageSize?: number;
}
