export interface MachineJobSummaryDto {
  id: number;
  partNumber: string;
  dueDate: string;
  lotNumber: string | null;
  startedDateTime: string | null;
}

export interface MachineWithJobsDto {
  id: number;
  serialNumber: string;
  modelNumber: string;
  jobs: MachineJobSummaryDto[];
}
