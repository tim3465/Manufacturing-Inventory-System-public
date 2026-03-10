export interface OrderProductionDto {
  id: number;
  customerName: string;
  partName: string;
  partNumber: string;
  partAmountRequested: number;
  partAmountCompleted: number;
  percentComplete: number;
}
