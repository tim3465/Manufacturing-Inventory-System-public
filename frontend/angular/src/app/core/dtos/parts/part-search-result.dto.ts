import { PartDto } from './part.dto';

export interface PartSearchResultDto {
  items: PartDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
