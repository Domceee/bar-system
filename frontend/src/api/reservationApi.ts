import type { ReservationFormData, ReservationListItem, ReservationProposal, ReservationResponse } from '../types/reservation';

const BASE = 'http://localhost:5029/api/reservation';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json() as Promise<T>;
}

export const submitReservation = (dto: ReservationFormData): Promise<ReservationProposal> =>
  fetch(`${BASE}/submit`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  }).then(handleResponse<ReservationProposal>);

export const confirmReservation = (dto: {
  barId: number;
  tableIds: number[];
  guestCount: number;
  date: string;
}): Promise<ReservationProposal> =>
  fetch(`${BASE}/confirm`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  }).then(handleResponse<ReservationProposal>);

export const getReservations = (): Promise<ReservationListItem[]> =>
  fetch(BASE).then(handleResponse<ReservationListItem[]>);

export const deleteReservation = (id: number): Promise<ReservationResponse> =>
  fetch(`${BASE}/${id}`, { method: 'DELETE' }).then(handleResponse<ReservationResponse>);

export const cancelReservation = (id: number): Promise<ReservationResponse> =>
  fetch(`${BASE}/${id}/cancel`, { method: 'PUT' }).then(handleResponse<ReservationResponse>);

export const endReservation = (id: number): Promise<ReservationResponse> =>
  fetch(`${BASE}/${id}/end`, { method: 'PUT' }).then(handleResponse<ReservationResponse>);
