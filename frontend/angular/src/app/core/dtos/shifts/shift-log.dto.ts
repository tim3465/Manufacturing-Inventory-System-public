export interface ShiftLogDto {
  id: number;
  machineSerialNumber: string;
  jobNumber: string;
  partNumber: string;
  startTime: string;
  stopTime: string | null;
  partsMade: number;
  scrap: number;
}
