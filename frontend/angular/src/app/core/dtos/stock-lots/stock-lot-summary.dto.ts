import { StockLotCondition } from '../shipping-receiving';

export interface StockLotSummaryDto {
  id: number;
  lotNumber: string;
  amountOfBars: number;
  diameter: number;
  barLength: number;
  condition: StockLotCondition;
  checkedInDateTime: string;
}
