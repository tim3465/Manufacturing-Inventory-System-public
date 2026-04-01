import { MyJobListItemDto } from './my-job.dto';

export interface MyJobSearchResultDto {
  items: MyJobListItemDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
