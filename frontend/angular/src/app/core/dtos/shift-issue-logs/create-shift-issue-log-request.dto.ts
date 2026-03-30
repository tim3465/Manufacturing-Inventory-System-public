export interface CreateShiftIssueLogRequestDto {
  shiftId: number;
  issueType: IssueType;
  scrapQuantity: number;
  description: string;
  downtime: string | null;
}

export type IssueType = 1 | 2;

export const ISSUE_TYPE_LABELS: Record<IssueType, string> = {
  1: 'Setup',
  2: 'Production'
};
