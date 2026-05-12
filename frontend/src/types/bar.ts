export interface Bar {
  id: number;
  name: string;
  xCoord: number;
  yCoord: number;
  rating: number;
  address: string;
  openTime: string;
  closeTime: string;
}

export interface CreateBarDto {
  name: string;
  xCoord: number;
  yCoord: number;
  rating: number;
  address: string;
  openTime: string;
  closeTime: string;
}

export interface UpdateBarDto {
  name: string;
  xCoord: number;
  yCoord: number;
  rating: number;
  address: string;
  openTime: string;
  closeTime: string;
}
