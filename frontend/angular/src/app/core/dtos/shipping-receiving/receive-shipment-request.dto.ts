export interface ReceiveShipmentRequestDto {
  materialId?: number | null;
  heatNumber?: string | null;
  materialName?: string | null;
  lotNumber: string;
  amountOfBars: number;
  diameter: number;
  barLength: number;
  condition: StockLotCondition;
  checkedInDateTime: string;
  notes?: string | null;
}

export type StockLotCondition = 1 | 2 | 3;

export const STOCK_LOT_CONDITIONS: StockLotCondition[] = [1, 2, 3];

export const STOCK_LOT_CONDITION_LABELS: Record<StockLotCondition, string> = {
  1: 'As Received',
  2: 'Ground',
  3: 'Turned'
};
