export interface BarInRoute {
  id: number;
  barId: number;
  barName: string;
  address: string;
  order: number;
  isLast: boolean;
  isCompleted: boolean;
}

export interface Route {
  id: number;
  status: string;
  bars: BarInRoute[];
}
