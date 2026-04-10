import { ShiftDto } from '../shifts/shift.dto';

export interface JobProductionDto {
  id: number;
  orderId: number;
  dueDate: string;
  machineId: number;
  machineName: string;
  partAmountPlanned: number;
  partName: string;
  partNumber: string;
  partsCompleted: number;
  percentComplete: number;
  shifts: ShiftDto[];
  stockLotId: number | null;
  lotNumber: string | null;
}
