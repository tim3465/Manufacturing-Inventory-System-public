export interface UpdateShiftRequestDto {
  startTime: string;
  stopTime: string | null;
  partsMade: number;
  barsConsumed: number;
  partsPerBar: number | null;
}
