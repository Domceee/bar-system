import type { Bar } from './bar';

export interface ReservationFormData {
  barId?: number;
  guestCount: number;
  date: string;
  userLat: number;
  userLon: number;
  useNearestBar: boolean;
}

export interface ReservationProposal {
  status: string;
  bar?: Bar;
  tableIds?: number[];
  weatherForecast?: string;
  errorMessage?: string;
}
