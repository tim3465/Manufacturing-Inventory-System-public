export interface MaterialSearchRequestDto {
  heatNumber?: string;
  materialName?: string;
  sortColumn: string;
  sortDirection: string;
  page: number;
  pageSize: number;
}
