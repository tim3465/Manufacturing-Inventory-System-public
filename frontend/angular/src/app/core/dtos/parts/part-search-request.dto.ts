export interface PartSearchRequestDto {
  partName?: string;
  partNumber?: string;
  sortColumn: string;
  sortDirection: string;
  page: number;
  pageSize: number;
}
