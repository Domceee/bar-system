import type { Friend } from '../types/friends';

const BASE = 'http://localhost:5029/api/friend';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json() as Promise<T>;
}

export const fetchFriends = (userId: number): Promise<Friend[]> =>
  fetch(`${BASE}/${userId}`).then(handleResponse<Friend[]>);

export const submitFriendRequest = (requesterId: number, targetUsername: string): Promise<Friend> =>
  fetch(`${BASE}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ requesterId, targetUsername }),
  }).then(handleResponse<Friend>);

export const acceptFriend = (requestId: number, userId: number): Promise<Friend> =>
  fetch(`${BASE}/accept`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ requestId, userId }),
  }).then(handleResponse<Friend>);

export const removeFriend = (friendId: number): Promise<void> =>
  fetch(`${BASE}/${friendId}`, { method: 'DELETE' }).then(res => {
    if (!res.ok) throw new Error(res.statusText);
  });

export const sendInvite = (senderId: number, friendUserId: number): Promise<void> =>
  fetch(`${BASE}/invite`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ senderId, friendUserId }),
  }).then(res => {
    if (!res.ok) throw new Error(res.statusText);
  });
