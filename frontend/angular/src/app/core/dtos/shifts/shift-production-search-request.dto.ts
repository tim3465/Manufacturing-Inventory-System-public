export interface ShiftProductionSearchRequestDto {
  operatorName?: string;
  jobNumber?: string;
  startTimeFrom?: string;
  startTimeTo?: string;
  stopTimeFrom?: string;
  stopTimeTo?: string;
  sortColumn: string;
  sortDirection: string;
  page: number;
  pageSize: number;
}
