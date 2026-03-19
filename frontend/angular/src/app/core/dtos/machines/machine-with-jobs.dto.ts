export interface MachineJobSummaryDto {
  id: number;
  partNumber: string;
  dueDate: string;
  lotNumber: string | null;
  startedDateTime: string | null;
  barsInJob: number;
  barAmountPlanned: number;
  runningShiftId: number | null;
}

export interface MachineWithJobsDto {
  id: number;
  serialNumber: string;
  modelNumber: string;
  jobs: MachineJobSummaryDto[];
}
