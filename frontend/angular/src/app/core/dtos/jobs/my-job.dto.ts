export interface MyJobListItemDto {
  id: number;
  jobNumber: string;
  partNumber: string;
  partName: string;
  machineName: string;
  endedDateTime: string | null;
}
