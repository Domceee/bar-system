export interface Bar {
  id: number;
  name: string;
  xCoord: number;
  yCoord: number;
}

export interface CreateBarDto {
  name: string;
  xCoord: number;
  yCoord: number;
}

export interface UpdateBarDto {
  name: string;
  xCoord: number;
  yCoord: number;
}
