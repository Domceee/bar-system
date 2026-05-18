import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Friend } from '../types/friends';
import { fetchFriends, acceptFriend, removeFriend } from '../api/friendApi';
import FriendAddForm from '../components/FriendAddForm';
import MessageWindow from '../components/MessageWindow';

const CURRENT_USER_ID = 99999;

export default function FriendsPage() {
  const navigate = useNavigate();
  const [friends, setFriends] = useState<Friend[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showAddForm, setShowAddForm] = useState(false);
  const [selectedFriend, setSelectedFriend] = useState<Friend | null>(null);
  const [removingFriend, setRemovingFriend] = useState<Friend | null>(null);

  useEffect(() => {
    open();
  }, []);

  async function open() {
    try {
      const data = await fetchFriends(CURRENT_USER_ID);
      setFriends(data);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to load friends.');
    } finally {
      setLoading(false);
    }
  }

  function openFriendAddForm() {
    setShowAddForm(true);
  }

  function selectSendMessage(friend: Friend) {
    openMessageWindow(friend);
  }

  function openMessageWindow(friend: Friend) {
    setSelectedFriend(friend);
  }

  function selectRemoveFriend(friend: Friend) {
    setRemovingFriend(friend);
  }

  async function confirmRemoveFriend(friendId: number) {
    try {
      await removeFriend(friendId);
      setFriends(prev => prev.filter(f => f.id !== friendId));
      if (selectedFriend?.id === friendId) setSelectedFriend(null);
      setRemovingFriend(null);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to remove friend.');
      setRemovingFriend(null);
    }
  }

  function cancel() {
    setRemovingFriend(null);
  }

  async function confirmFriendRequest(requestId: number) {
    try {
      const updated = await acceptFriend(requestId, CURRENT_USER_ID);
      setFriends(prev => prev.map(f => f.id === requestId ? { ...f, status: updated.status } : f));
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to accept request.');
    }
  }

  const acceptedFriends = friends.filter(f => f.status === 'Accepted');
  const pendingRequests = friends.filter(f => f.status === 'Pending');

  return (
    <div className="friends-page">
      <div className="friends-layout">
        <div className="friends-panel">
          <div className="friends-panel__header">
            <button className="btn btn--ghost" onClick={() => navigate('/')}>← Back</button>
            <h1 className="friends-panel__title">Friends</h1>
            <button className="btn btn--primary" onClick={openFriendAddForm}>+ Add</button>
          </div>

          {showAddForm && (
            <FriendAddForm
              currentUserId={CURRENT_USER_ID}
              onAdded={f => { setFriends(prev => [...prev, f]); setShowAddForm(false); }}
              onClose={() => setShowAddForm(false)}
            />
          )}

          {error && <p className="bar-form__error" style={{ margin: '0 12px 8px' }}>{error}</p>}

          {loading ? (
            <p className="friends-empty">Loading…</p>
          ) : (
            <>
              {pendingRequests.length > 0 && (
                <div className="friends-section">
                  <p className="friends-section__label">Pending Requests</p>
                  <ul className="friends-list">
                    {pendingRequests.map(f => (
                      <li key={f.id} className="friends-list__item friends-list__item--pending">
                        <span className="friends-list__name">{f.friendUsername}</span>
                        <button className="btn btn--primary" onClick={() => confirmFriendRequest(f.id)}>
                          Accept
                        </button>
                      </li>
                    ))}
                  </ul>
                </div>
              )}

              {acceptedFriends.length > 0 ? (
                <div className="friends-section">
                  <p className="friends-section__label">Friends</p>
                  <ul className="friends-list">
                    {acceptedFriends.map(f => (
                      <li
                        key={f.id}
                        className={`friends-list__item${selectedFriend?.id === f.id ? ' friends-list__item--active' : ''}`}
                        onClick={() => selectSendMessage(f)}
                      >
                        <span className="friends-list__name">{f.friendUsername}</span>
                        <button
                          className="btn btn--delete friends-list__remove"
                          onClick={e => { e.stopPropagation(); selectRemoveFriend(f); }}
                        >
                          ✕
                        </button>
                      </li>
                    ))}
                  </ul>
                </div>
              ) : (
                !pendingRequests.length && <p className="friends-empty">No friends yet. Add someone!</p>
              )}
            </>
          )}
        </div>

        <div className="message-panel">
          {selectedFriend ? (
            <MessageWindow friend={selectedFriend} onClose={() => setSelectedFriend(null)} />
          ) : (
            <div className="message-panel__empty">
              <p>Select a friend to start chatting</p>
            </div>
          )}
        </div>
      </div>

      {removingFriend && (
        <div className="confirm-overlay">
          <div className="confirm-dialog">
            <p className="confirm-dialog__text">
              Remove <strong>{removingFriend.friendUsername}</strong> from friends?
            </p>
            <div className="confirm-dialog__actions">
              <button className="btn btn--delete" onClick={() => confirmRemoveFriend(removingFriend.id)}>
                Remove
              </button>
              <button className="btn btn--ghost" onClick={cancel}>
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
