export interface CreateJobInOrderRequestDto {
  stockLotId: number;
  machineId: number;
  partAmountPlanned: number;
  barAmountPlanned: number;
  barCycleTime: string;
  barsInJob: number;
  estimatedPartsPerBar: number | null;
  dueDate: string;
}

export interface CreateOrderWithJobsRequestDto {
  customerId: number;
  partId: number;
  partAmountRequested: number;
  partsPerBar: number;
  jobs: CreateJobInOrderRequestDto[];
}

export interface CreateOrderWithJobsResponseDto {
  orderId: number;
  jobIds: number[];
}
