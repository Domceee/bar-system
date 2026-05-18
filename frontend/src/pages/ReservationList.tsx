import { useEffect, useState } from 'react';
import { getReservations, deleteReservation, cancelReservation, endReservation } from '../api/reservationApi';
import type { ReservationListItem } from '../types/reservation';
import ReservationForm from './ReservationForm';

type View = 'list' | 'create';

export default function ReservationList() {
  const [view, setView] = useState<View>('list');
  const [reservations, setReservations] = useState<ReservationListItem[]>([]);
  const [pendingDeleteId, setPendingDeleteId] = useState<number | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  function openReservations() {
    getReservations().then(setReservations);
  }

  useEffect(() => {
    openReservations();
  }, []);

  function selectDeleteReservation(id: number) {
    setPendingDeleteId(id);
    setMessage(null);
    setError(null);
  }

  async function deleteReservationHandler() {
    if (pendingDeleteId === null) return;
    setPendingDeleteId(null);
    try {
      const result = await deleteReservation(pendingDeleteId);
      if (result.status === 'deleted' && result.reservations) {
        setReservations(result.reservations);
      } else if (result.status === 'cancelled') {
        setMessage(result.message ?? null);
        openReservations();
      }
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Cannot delete active reservation');
    }
  }

  function cancel() {
    setPendingDeleteId(null);
    openReservations();
  }

  async function cancelReservationHandler(id: number) {
    setMessage(null);
    setError(null);
    const result = await cancelReservation(id);
    setMessage(result.message ?? null);
    openReservations();
  }

  async function endReservationHandler(id: number) {
    setMessage(null);
    setError(null);
    const result = await endReservation(id);
    setMessage(result.message ?? null);
    openReservations();
  }

  if (view === 'create') {
    return (
      <ReservationForm
        onCancel={() => { setView('list'); openReservations(); }}
        onConfirmed={() => { setView('list'); openReservations(); }}
      />
    );
  }

  return (
    <div className="bar-form-page">
      <div style={{ width: '100%', maxWidth: 800, margin: '0 auto', padding: '2rem' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
          <h2 className="bar-form__title" style={{ margin: 0 }}>Reservations</h2>
          <button className="btn btn--primary" onClick={() => setView('create')}>+ New Reservation</button>
        </div>

        {message && <p style={{ color: 'green', marginBottom: '1rem' }}>{message}</p>}
        {error && <p style={{ color: 'red', marginBottom: '1rem' }}>{error}</p>}

        {reservations.length === 0 ? (
          <p>No reservations found.</p>
        ) : (
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid #ccc', textAlign: 'left' }}>
                <th style={{ padding: '0.5rem' }}>ID</th>
                <th style={{ padding: '0.5rem' }}>Bar</th>
                <th style={{ padding: '0.5rem' }}>Guests</th>
                <th style={{ padding: '0.5rem' }}>Date</th>
                <th style={{ padding: '0.5rem' }}>Status</th>
                <th style={{ padding: '0.5rem' }}>Tables</th>
                <th style={{ padding: '0.5rem' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {reservations.map(r => (
                <tr key={r.id} style={{ borderBottom: '1px solid #eee' }}>
                  <td style={{ padding: '0.5rem' }}>{r.id}</td>
                  <td style={{ padding: '0.5rem' }}>{r.barName}</td>
                  <td style={{ padding: '0.5rem' }}>{r.guestCount}</td>
                  <td style={{ padding: '0.5rem' }}>{new Date(r.date).toLocaleString()}</td>
                  <td style={{ padding: '0.5rem' }}>{r.status}</td>
                  <td style={{ padding: '0.5rem' }}>{r.tableIds.join(', ')}</td>
                  <td style={{ padding: '0.5rem', display: 'flex', gap: '0.5rem' }}>
                    <button className="btn btn--ghost" onClick={() => cancelReservationHandler(r.id)}>Cancel</button>
                    <button className="btn btn--ghost" onClick={() => endReservationHandler(r.id)}>End</button>
                    <button className="btn btn--ghost" onClick={() => selectDeleteReservation(r.id)}>Delete</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        {pendingDeleteId !== null && (
          <div style={{
            position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.4)',
            display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100
          }}>
            <div className="bar-form" style={{ maxWidth: 400 }}>
              <h3>Confirm Delete</h3>
              <p>Are you sure you want to delete reservation #{pendingDeleteId}?</p>
              <div className="bar-form__actions">
                <button className="btn btn--ghost" onClick={cancel}>Cancel</button>
                <button className="btn btn--primary" onClick={deleteReservationHandler}>Delete</button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
