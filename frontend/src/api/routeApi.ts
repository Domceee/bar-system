import type { Route, BarInRoute } from '../types/route';

const BASE = 'http://localhost:5029/api';

async function handle<T>(res: Response): Promise<T> {
  if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
  return res.json();
}

export const requestFittingBars = (userIds: number[]): Promise<Route> =>
  fetch(`${BASE}/bar/fitting-bars`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userIds }),
  }).then(handle<Route>);

export const generateRoute = (routeId: number): Promise<Route> =>
  fetch(`${BASE}/route/generate/${routeId}`, { method: 'POST' }).then(handle<Route>);

export const startRoute = (routeId: number): Promise<Route> =>
  fetch(`${BASE}/route/start/${routeId}`, { method: 'POST' }).then(handle<Route>);

export const cancelRoute = (routeId: number): Promise<Route> =>
  fetch(`${BASE}/route/cancel/${routeId}`, { method: 'POST' }).then(handle<Route>);

export const getBarInRoute = (routeId: number): Promise<BarInRoute> =>
  fetch(`${BASE}/route/${routeId}/current`).then(handle<BarInRoute>);

export const abortRoute = (routeId: number): Promise<Route> =>
  fetch(`${BASE}/route/abort/${routeId}`, { method: 'POST' }).then(handle<Route>);

export const updateRoute = (routeId: number): Promise<Route> =>
  fetch(`${BASE}/route/update/${routeId}`, { method: 'POST' }).then(handle<Route>);
