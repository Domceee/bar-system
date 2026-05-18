import { useState } from 'react';
import { submitReservation, confirmReservation } from '../api/reservationApi';
import type { ReservationFormData, ReservationProposal } from '../types/reservation';

type Step = 'form' | 'barClosed' | 'proposal' | 'confirmed' | 'error' | 'noBarsFound';

interface Props {
  onCancel: () => void;
  onConfirmed: () => void;
}

export default function ReservationForm({ onCancel, onConfirmed }: Props) {
  const [step, setStep] = useState<Step>('form');
  const [formData, setFormData] = useState<ReservationFormData>({
    guestCount: 1,
    date: '',
    userLat: 0,
    userLon: 0,
    useNearestBar: false,
  });
  const [proposal, setProposal] = useState<ReservationProposal | null>(null);
  const [weather, setWeather] = useState<string | null>(null);

  function reset() {
    setStep('form');
    setProposal(null);
    setWeather(null);
    setFormData({ guestCount: 1, date: '', userLat: 0, userLon: 0, useNearestBar: false });
  }

  async function submit(data: ReservationFormData) {
    const result = await submitReservation(data);
    setProposal(result);
    if (result.status === 'found') setStep('proposal');
    else if (result.status === 'barClosed') setStep('barClosed');
    else if (result.status === 'noBarsFound') setStep('noBarsFound');
    else setStep('error');
  }

  async function confirm() {
    if (!proposal?.bar || !proposal.tableIds) return;
    const result = await confirmReservation({
      barId: proposal.bar.id,
      tableIds: proposal.tableIds,
      guestCount: formData.guestCount,
      date: formData.date,
    });
    setWeather(result.weatherForecast ?? null);
    setStep('confirmed');
  }

  if (step === 'form') {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Make a <span>Reservation</span></h2>
          <div className="bar-form__field">
            <label className="bar-form__label">Guest Count</label>
            <input className="bar-form__input" type="number" min={1} value={formData.guestCount}
              onChange={e => setFormData(f => ({ ...f, guestCount: +e.target.value }))} />
          </div>
          <div className="bar-form__field">
            <label className="bar-form__label">Date & Time</label>
            <input className="bar-form__input" type="datetime-local" value={formData.date}
              onChange={e => setFormData(f => ({ ...f, date: e.target.value }))} />
          </div>
          <div className="bar-form__field">
            <label className="bar-form__label">Your Latitude</label>
            <input className="bar-form__input" type="number" step="any" value={formData.userLat}
              onChange={e => setFormData(f => ({ ...f, userLat: +e.target.value }))} />
          </div>
          <div className="bar-form__field">
            <label className="bar-form__label">Your Longitude</label>
            <input className="bar-form__input" type="number" step="any" value={formData.userLon}
              onChange={e => setFormData(f => ({ ...f, userLon: +e.target.value }))} />
          </div>
          <div className="bar-form__field">
            <label className="bar-form__label">Preferred Bar ID (optional)</label>
            <input className="bar-form__input" type="number" value={formData.barId ?? ''}
              onChange={e => setFormData(f => ({ ...f, barId: e.target.value ? +e.target.value : undefined }))} />
          </div>
          <div className="bar-form__actions">
            <button className="btn btn--ghost" onClick={onCancel}>Cancel</button>
            <button className="btn btn--primary" onClick={() => submit(formData)}>Submit</button>
          </div>
        </div>
      </div>
    );
  }

  if (step === 'barClosed') {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Bar is <span>Closed</span></h2>
          <p>Would you like us to find the nearest open bar?</p>
          <div className="bar-form__actions">
            <button className="btn btn--ghost" onClick={reset}>Cancel</button>
            <button className="btn btn--primary" onClick={() => submit({ ...formData, useNearestBar: true })}>Find Nearest Bar</button>
          </div>
        </div>
      </div>
    );
  }

  if (step === 'proposal' && proposal?.bar) {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Reservation <span>Proposal</span></h2>
          <p><strong>Bar:</strong> {proposal.bar.name}</p>
          <p><strong>Address:</strong> {proposal.bar.address}</p>
          <p><strong>Rating:</strong> {proposal.bar.rating}</p>
          <p><strong>Table IDs:</strong> {proposal.tableIds?.join(', ')}</p>
          <div className="bar-form__actions">
            <button className="btn btn--ghost" onClick={reset}>Decline</button>
            <button className="btn btn--primary" onClick={confirm}>Confirm</button>
          </div>
        </div>
      </div>
    );
  }

  if (step === 'confirmed') {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">Reservation <span>Confirmed</span></h2>
          {weather && <p><strong>Weather forecast:</strong> {weather}</p>}
          <div className="bar-form__actions">
            <button className="btn btn--primary" onClick={onConfirmed}>Back to List</button>
          </div>
        </div>
      </div>
    );
  }

  if (step === 'noBarsFound') {
    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">No Bars <span>Found</span></h2>
          <p>No available bars found for your request.</p>
          <div className="bar-form__actions">
            <button className="btn btn--ghost" onClick={onCancel}>Back</button>
            <button className="btn btn--primary" onClick={reset}>Try Again</button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bar-form-page">
      <div className="bar-form">
        <h2 className="bar-form__title">Something went <span>Wrong</span></h2>
        <p>{proposal?.errorMessage ?? 'An error occurred.'}</p>
        <div className="bar-form__actions">
          <button className="btn btn--ghost" onClick={onCancel}>Back</button>
          <button className="btn btn--primary" onClick={reset}>Try Again</button>
        </div>
      </div>
    </div>
  );
}
