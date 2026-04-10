export interface RunningShiftDto {
  id: number;
  jobId: number;
  machineId: number;
  machineSerialNumber: string;
  partName: string;
  partNumber: string;
  jobTotalPartsMade: number;
  jobTotalScrap: number;
  jobTotalBarsConsumed: number;
  startTime: string;
  stopTime: string | null;
  partsMade: number;
  scrap: number;
  barsConsumed: number;
  partsPerBar: number | null;
  downtime: string | null;
}
