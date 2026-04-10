import { MaterialDto } from './material.dto';

export interface MaterialSearchResultDto {
  items: MaterialDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
