import type { Bar, CreateBarDto, UpdateBarDto } from '../types/bar';
import type { BarsWithinDistanceResponse } from '../types/barRec';

const BASE = 'http://localhost:5029/api/bar';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json() as Promise<T>;
}

export const fetchBars = (): Promise<Bar[]> =>
  fetch(BASE).then(handleResponse<Bar[]>);

export const fetchThisBar = (id: number): Promise<Bar> =>
  fetch(`${BASE}/${id}`).then(handleResponse<Bar>);

export const createBar = (dto: CreateBarDto): Promise<Bar> =>
  fetch(BASE, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  }).then(handleResponse<Bar>);

export const updateBar = (id: number, dto: UpdateBarDto): Promise<Bar> =>
  fetch(`${BASE}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  }).then(handleResponse<Bar>);

export const deleteBar = async (id: number): Promise<void> => {
  const res = await fetch(`${BASE}/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error(await res.text() || res.statusText);
};

export const requestBarsWithinDistance = (
  userId: number,
  lat: number,
  lon: number,
  distanceMeters: number,
): Promise<BarsWithinDistanceResponse> =>
  fetch(`${BASE}/within-distance`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userId, lat, lon, distanceMeters }),
  }).then(handleResponse<BarsWithinDistanceResponse>);
