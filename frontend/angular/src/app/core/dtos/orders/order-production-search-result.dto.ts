import { OrderProductionDto } from './order-production.dto';

export interface OrderProductionSearchResultDto {
  items: OrderProductionDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
