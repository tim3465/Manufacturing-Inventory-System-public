import { StockLotSummaryDto } from './stock-lot-summary.dto';

export interface StockLotSearchResultDto {
  items: StockLotSummaryDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
