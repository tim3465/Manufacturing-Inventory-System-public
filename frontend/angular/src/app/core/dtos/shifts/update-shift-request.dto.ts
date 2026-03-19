export interface UpdateShiftRequestDto {
  startTime: string;
  stopTime: string | null;
  partsMade: number;
  scrap: number;
  barsConsumed: number;
  partsPerBar: number | null;
  downtime: string | null;
}
