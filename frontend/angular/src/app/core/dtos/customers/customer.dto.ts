export interface CustomerDto {
  id: number;
  companyName: string;
  phone: string;
  email: string;
  address: string;
}

export interface CreateCustomerRequestDto {
  companyName: string;
  phone: string;
  email: string;
  address: string;
}

export interface UpdateCustomerRequestDto {
  companyName: string;
  phone: string;
  email: string;
  address: string;
}
