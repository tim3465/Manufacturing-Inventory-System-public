import { ShiftDto } from '../shifts/shift.dto';

export interface MyJobDto {
  id: number;
  jobNumber: string;
  partNumber: string;
  partName: string;
  machineName: string;
  endedDateTime: string | null;
  shifts: ShiftDto[];
}
