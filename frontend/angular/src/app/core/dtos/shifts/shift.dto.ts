export interface ShiftDto {
  id: number;
  jobId: number;
  operatorId: number;
  operatorName: string;
  barsConsumed: number;
  partsMade: number;
  scrap: number;
  partsPerBar: number | null;
  startTime: string;
  stopTime: string | null;
  downtime: string | null;
  partName: string;
  partNumber: string;
}
