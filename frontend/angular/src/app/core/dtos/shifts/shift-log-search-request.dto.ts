export interface ShiftLogSearchRequestDto {
  machineName?: string;
  jobNumber?: string;
  partNumber?: string;
  startTimeFrom?: string;
  startTimeTo?: string;
  stopTimeFrom?: string;
  stopTimeTo?: string;
  sortColumn: string;
  sortDirection: string;
  page: number;
  pageSize: number;
}
