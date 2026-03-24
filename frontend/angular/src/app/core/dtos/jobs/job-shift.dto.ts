export interface JobShiftDto {
  shiftId: number;
  machinistName: string;
  startDateTime: string;
  endDateTime: string | null;
}
