export interface MachineDto {
  id: number;
  serialNumber: string;
  modelNumber: string;
  inactivatedDateTime: string | null;
}

export interface CreateMachineRequestDto {
  serialNumber: string;
  modelNumber: string;
}
