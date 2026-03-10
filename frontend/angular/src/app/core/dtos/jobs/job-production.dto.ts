import { ShiftDto } from '../shifts/shift.dto';

export interface JobProductionDto {
  id: number;
  orderId: number;
  dueDate: string;
  machineId: number;
  partAmountPlanned: number;
  shifts: ShiftDto[];
}
