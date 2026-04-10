import { JobProductionDto } from './job-production.dto';

export interface JobProductionSearchResultDto {
  items: JobProductionDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
