import { StockLotCondition } from '../shipping-receiving';

export interface StockLotSearchRequestDto {
  lotNumber?: string;
  checkedInFrom?: string;
  checkedInTo?: string;
  diameterExact?: number;
  diameterMin?: number;
  diameterMax?: number;
  condition?: StockLotCondition;
  sortColumn?: string;
  sortDirection?: string;
  page?: number;
  pageSize?: number;
}
