import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Route, BarInRoute } from '../types/route';
import type { Friend } from '../types/friends';
import { fetchFriends, sendInvite } from '../api/friendApi';
import {
  requestFittingBars,
  generateRoute,
  startRoute,
  cancelRoute,
  getBarInRoute,
  abortRoute,
  updateRoute,
} from '../api/routeApi';

const USER_ID = 99999;

type Phase = 'select' | 'preview' | 'active' | 'done';

export default function BarRoutePage() {
  const navigate = useNavigate();
  const [friends, setFriends] = useState<Friend[]>([]);
  const [selectedFriendIds, setSelectedFriendIds] = useState<number[]>([]);
  const [route, setRoute] = useState<Route | null>(null);
  const [currentBar, setCurrentBar] = useState<BarInRoute | null>(null);
  const [phase, setPhase] = useState<Phase>('select');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showCancelConfirm, setShowCancelConfirm] = useState(false);

  useEffect(() => {
    fetchFriends(USER_ID).then(setFriends).catch(() => {});
  }, []);

  function toggleFriend(friendUserId: number) {
    setSelectedFriendIds(prev =>
      prev.includes(friendUserId)
        ? prev.filter(id => id !== friendUserId)
        : [...prev, friendUserId]
    );
  }

  async function openBarRoutePage() {
    setLoading(true);
    setError(null);
    try {
      const userIds = [USER_ID, ...selectedFriendIds];
      const draft = await requestFittingBars(userIds);
      const optimized = await generateRoute(draft.id);
      setRoute(optimized);
      for (const friendUserId of selectedFriendIds) {
        await sendInvite(USER_ID, friendUserId).catch(() => {});
      }
      setPhase('preview');
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to generate route.');
    } finally {
      setLoading(false);
    }
  }

  async function startRoute_(routeId: number) {
    setLoading(true);
    setError(null);
    try {
      const started = await startRoute(routeId);
      setRoute(started);
      const bar = await getBarInRoute(routeId);
      setCurrentBar(bar);
      setPhase('active');
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to start route.');
    } finally {
      setLoading(false);
    }
  }

  async function cancelRoute_(routeId: number) {
    await cancelRoute(routeId).catch(() => {});
    navigate('/');
  }

  async function hasArrived(routeId: number) {
    setLoading(true);
    setError(null);
    try {
      const updated = await updateRoute(routeId);
      setRoute(updated);
      if (updated.status === 'Finished') {
        setCurrentBar(null);
        setPhase('done');
      } else {
        const bar = await getBarInRoute(routeId);
        setCurrentBar(bar);
      }
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to update route.');
    } finally {
      setLoading(false);
    }
  }

  async function cancelRouteTracking(routeId: number) {
    setLoading(true);
    try {
      const aborted = await abortRoute(routeId);
      setRoute(aborted);
      setCurrentBar(null);
      setPhase('done');
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to abort route.');
    } finally {
      setLoading(false);
    }
  }

  const accepted = friends.filter(f => f.status === 'Accepted');
  const completedCount = route?.bars.filter(b => b.isCompleted).length ?? 0;
  const totalCount = route?.bars.length ?? 0;

  if (phase === 'select') {
    return (
      <div className="rp-shell">
        <div className="rp-card">
          <div className="rp-header">
            <h1 className="rp-title">Bar Route</h1>
            <p className="rp-sub">We'll pick the best bars based on your taste profile.</p>
          </div>

          <div className="rp-section">
            <p className="rp-section__label">Invite friends</p>
            {accepted.length === 0 ? (
              <p className="rp-empty">You have no friends added yet.</p>
            ) : (
              <ul className="rp-friend-list">
                {accepted.map(f => (
                  <li
                    key={f.id}
                    className={`rp-friend-item ${selectedFriendIds.includes(f.friendUserId) ? 'rp-friend-item--selected' : ''}`}
                    onClick={() => toggleFriend(f.friendUserId)}
                  >
                    <span className="rp-friend-avatar">{f.friendUsername[0].toUpperCase()}</span>
                    <span className="rp-friend-name">{f.friendUsername}</span>
                    <span className="rp-friend-check">{selectedFriendIds.includes(f.friendUserId) ? '✓' : ''}</span>
                  </li>
                ))}
              </ul>
            )}
          </div>

          {error && <p className="rp-error">{error}</p>}

          <div className="rp-actions">
            <button className="btn btn--ghost" onClick={() => navigate('/')}>← Back</button>
            <button className="btn--primary-lg" onClick={openBarRoutePage} disabled={loading}>
              {loading ? 'Generating…' : 'Generate Route'}
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (phase === 'preview' && route) {
    return (
      <div className="rp-shell">
        <div className="rp-card">
          <div className="rp-header">
            <h1 className="rp-title">Your Route</h1>
            <p className="rp-sub">{route.bars.length} stops planned for tonight.</p>
          </div>

          <ol className="rp-stops">
            {route.bars.map((b, i) => (
              <li key={b.id} className="rp-stop">
                <span className="rp-stop__num">{i + 1}</span>
                <div className="rp-stop__info">
                  <span className="rp-stop__name">{b.barName}</span>
                  <span className="rp-stop__addr">{b.address}</span>
                </div>
                {b.isLast && <span className="rp-stop__badge">Last</span>}
              </li>
            ))}
          </ol>

          {error && <p className="rp-error">{error}</p>}

          <div className="rp-actions">
            <button className="btn btn--ghost" onClick={() => setShowCancelConfirm(true)}>Cancel</button>
            <button className="btn--primary-lg" onClick={() => startRoute_(route.id)} disabled={loading}>
              {loading ? 'Starting…' : 'Start Route'}
            </button>
          </div>
        </div>

          {showCancelConfirm && (
            <div className="confirm-overlay">
              <div className="confirm-dialog">
                <p className="confirm-dialog__text">Cancel this route?</p>
                <div className="confirm-dialog__actions">
                  <button className="btn btn--delete" onClick={() => { setShowCancelConfirm(false); cancelRoute_(route.id); }}>
                    Yes, cancel
                  </button>
                  <button className="btn btn--ghost" onClick={() => setShowCancelConfirm(false)}>
                    Go back
                  </button>
                </div>
              </div>
            </div>
          )}
      </div>
    );
  }

  if (phase === 'active' && route && currentBar) {
    return (
      <div className="rp-shell">
        <div className="rp-card">
          <div className="rp-header">
            <h1 className="rp-title">On Route</h1>
            <div className="rp-progress">
              <div className="rp-progress__bar" style={{ width: `${(completedCount / totalCount) * 100}%` }} />
            </div>
            <p className="rp-sub">{completedCount} of {totalCount} stops done</p>
          </div>

          <div className="rp-current">
            <p className="rp-current__eyebrow">Head to</p>
            <p className="rp-current__name">{currentBar.barName}</p>
            <p className="rp-current__addr">{currentBar.address}</p>
            {currentBar.isLast && <span className="rp-current__badge">Last stop!</span>}
          </div>

          {error && <p className="rp-error">{error}</p>}

          <div className="rp-actions">
            <button className="btn--secondary" onClick={() => cancelRouteTracking(route.id)} disabled={loading}>
              End Early
            </button>
            <button className="btn--primary-lg" onClick={() => hasArrived(route.id)} disabled={loading}>
              {loading ? '…' : 'Arrived'}
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (phase === 'done') {
    return (
      <div className="rp-shell">
        <div className="rp-card rp-card--done">
          <div className="rp-done-icon">🍻</div>
          <h1 className="rp-title">Night's over!</h1>
          <p className="rp-sub">Hope you had a great time.</p>
          <button className="btn--primary-lg" onClick={() => navigate('/')}>Back to Home</button>
        </div>
      </div>
    );
  }

  return null;
}
