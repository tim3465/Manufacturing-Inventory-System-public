import { ShiftLogDto } from './shift-log.dto';

export interface ShiftLogSearchResultDto {
  items: ShiftLogDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
