import { StockLotCondition } from '../shipping-receiving';

export interface StockLotDto {
  id: number;
  lotNumber: string;
  materialId: number;
  amountOfBars: number;
  diameter: number;
  barLength: number;
  condition: StockLotCondition;
  checkedInDateTime: string;
}
