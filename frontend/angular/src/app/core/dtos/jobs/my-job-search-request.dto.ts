export interface MyJobSearchRequestDto {
  jobNumber?: string;
  partNumber?: string;
  machineName?: string;
  status?: string;
  sortColumn: string;
  sortDirection: string;
  page: number;
  pageSize: number;
}
