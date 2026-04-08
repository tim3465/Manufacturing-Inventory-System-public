export interface JobReportIssueLogDto {
  id: number;
  shiftId: number;
  operatorName: string;
  createdDateTime: string;
  issueType: number;
  description: string;
  scrapQuantity: number;
  downtime: string | null;
}

export interface JobReportShiftDto {
  id: number;
  operatorName: string;
  startTime: string;
  stopTime: string | null;
  partsMade: number;
  scrap: number;
  barsConsumed: number;
  partsPerBar: number | null;
  downtime: string | null;
}

export interface JobReportDto {
  id: number;
  orderId: number;
  machineName: string;
  partName: string;
  partNumber: string;
  dueDate: string;
  startedDateTime: string | null;
  endedDateTime: string | null;
  jobStatus: string;
  partAmountPlanned: number;
  totalPartsMade: number;
  totalScrap: number;
  barAmountPlanned: number;
  totalBarsConsumed: number;
  estimatedPartsPerBar: number | null;
  actualPartsPerBar: number | null;
  totalDowntime: string;
  totalUptime: string;
  shifts: JobReportShiftDto[];
  issueLogs: JobReportIssueLogDto[];
}
