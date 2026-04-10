import { ShiftDto } from './shift.dto';

export interface ShiftProductionSearchResultDto {
  items: ShiftDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
