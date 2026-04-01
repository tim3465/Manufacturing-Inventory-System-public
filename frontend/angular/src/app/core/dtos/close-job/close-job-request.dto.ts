import { UpdateShiftRequestDto } from '../shifts/update-shift-request.dto';

export interface CloseJobRequestDto {
  shiftId: number;
  jobId: number;
  shiftData: UpdateShiftRequestDto;
}
