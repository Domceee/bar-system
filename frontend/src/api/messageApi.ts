import type { Message } from '../types/friends';

const BASE = 'http://localhost:5029/api/message';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json() as Promise<T>;
}

export const fetch = (userId: number, friendId: number): Promise<Message[]> =>
  globalThis.fetch(`${BASE}/${userId}/${friendId}`).then(handleResponse<Message[]>);

export const send = (senderId: number, receiverId: number, content: string): Promise<Message> =>
  globalThis.fetch(`${BASE}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ senderId, receiverId, content }),
  }).then(handleResponse<Message>);
