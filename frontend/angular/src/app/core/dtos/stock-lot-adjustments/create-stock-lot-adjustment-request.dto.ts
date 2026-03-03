export interface CreateStockLotAdjustmentRequestDto {
  stockLotId: number;
  deltaBars: number;
  reason: AdjustmentReason;
  notes?: string | null;
}

export type AdjustmentReason = 2 | 3 | 4 | 5;

export const ADJUSTMENT_REASONS: AdjustmentReason[] = [2, 3, 4, 5];

export const ADJUSTMENT_REASON_LABELS: Record<AdjustmentReason, string> = {
  2: 'Consumed',
  3: 'Adjusted',
  4: 'Scrap',
  5: 'Return'
};
