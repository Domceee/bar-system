import type { BlackjackProfile } from '../types/blackjack';

const BASE = 'http://localhost:5029/api/blackjack';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json() as Promise<T>;
}

export const getProfile = (): Promise<BlackjackProfile> =>
  fetch(`${BASE}/profile`).then(handleResponse<BlackjackProfile>);

export const addChips = (amount: number): Promise<BlackjackProfile> =>
  fetch(`${BASE}/chips/add`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ amount }),
  }).then(handleResponse<BlackjackProfile>);

export const discardChips = (amount: number): Promise<BlackjackProfile> =>
  fetch(`${BASE}/chips/discard`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ amount }),
  }).then(handleResponse<BlackjackProfile>);
