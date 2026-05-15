export interface NearbyBar {
  name: string;
  lat: number;
  lon: number;
  placeId: string;
  rating: number | null;
}

export interface DbBar {
  id: number;
  name: string;
  xCoord: number;
  yCoord: number;
  rating: number;
  design: string;
  priority: number;
}

export interface BarsWithinDistanceResponse {
  status: string;
  bars: NearbyBar[];
  dbBars: DbBar[];
}
