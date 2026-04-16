import type { Bar, CreateBarDto, UpdateBarDto } from '../types/bar';

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

export const deleteBar = (id: number): Promise<void> =>
  fetch(`${BASE}/${id}`, { method: 'DELETE' }).then(handleResponse<void>);
