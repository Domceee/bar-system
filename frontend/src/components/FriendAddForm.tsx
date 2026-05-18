import { useState } from 'react';
import { submitFriendRequest } from '../api/friendApi';
import type { Friend } from '../types/friends';

interface Props {
  currentUserId: number;
  onAdded: (friend: Friend) => void;
  onClose: () => void;
}

export default function FriendAddForm({ currentUserId, onAdded, onClose }: Props) {
  const [username, setUsername] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function submit() {
    if (!username.trim()) return;
    setLoading(true);
    setError(null);
    try {
      const friend = await submitFriendRequest(currentUserId, username.trim());
      onAdded(friend);
      setUsername('');
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to send request.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="friend-add-menu">
      <div className="friend-add-menu__header">
        <span className="friend-add-menu__title">Add Friend</span>
        <button className="btn btn--ghost friend-add-menu__close" onClick={onClose}>✕</button>
      </div>
      <div className="friend-add-menu__body">
        <input
          className="bar-form__input"
          placeholder="Enter username"
          value={username}
          onChange={e => setUsername(e.target.value)}
          onKeyDown={e => e.key === 'Enter' && submit()}
          autoFocus
        />
        {error && <p className="bar-form__error">{error}</p>}
        <button
          className="btn btn--primary"
          onClick={submit}
          disabled={loading || !username.trim()}
        >
          {loading ? 'Sending…' : 'Send Request'}
        </button>
      </div>
    </div>
  );
}
