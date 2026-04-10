import { IssueType } from './create-shift-issue-log-request.dto';

export interface ShiftIssueLogDto {
  id: number;
  shiftId: number;
  issueType: IssueType;
  scrapQuantity: number;
  downtime: string | null; // "HH:MM:SS" or null
  description: string;
  createdDateTime: string; // ISO 8601 DateTimeOffset
  createdByUserId: number | null;
  createdByUserDisplayName: string;
}
