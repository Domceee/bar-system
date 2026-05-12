import { useState } from 'react';
import type { Bar, CreateBarDto } from '../types/bar';

interface Props {
  initial?: Bar;
  onSubmit: (data: CreateBarDto) => Promise<void>;
  onCancel: () => void;
  error: string | null;
}

export default function BarForm({ initial, onSubmit, onCancel, error }: Props) {
  const [name, setName] = useState(initial?.name ?? '');
  const [xCoord, setXCoord] = useState(initial?.xCoord?.toString() ?? '');
  const [yCoord, setYCoord] = useState(initial?.yCoord?.toString() ?? '');
  const [rating, setRating] = useState(initial?.rating?.toString() ?? '');
  const [address, setAddress] = useState(initial?.address ?? '');
  const [openTime, setOpenTime] = useState(initial?.openTime?.slice(0, 5) ?? '');
  const [closeTime, setCloseTime] = useState(initial?.closeTime?.slice(0, 5) ?? '');
  const [validationError, setValidationError] = useState<string | null>(null);

  function isDataValid(): boolean {
    if (!name.trim()) { setValidationError('Name is required.'); return false; }
    if (isNaN(Number(xCoord)) || isNaN(Number(yCoord))) { setValidationError('Coordinates must be valid numbers.'); return false; }
    if (!openTime || !closeTime) { setValidationError('Working hours are required.'); return false; }
    setValidationError(null);
    return true;
  }

  async function submitData() {
    if (!isDataValid()) return;
    await onSubmit({
      name: name.trim(),
      xCoord: Number(xCoord),
      yCoord: Number(yCoord),
      rating: Number(rating),
      address,
      openTime: openTime + ':00',
      closeTime: closeTime + ':00',
    });
  }

  const displayedError = validationError ?? error;

  return (
    <div className="bar-form-page">
      <div className="bar-form">
        <h2 className="bar-form__title">
          {initial ? <>Edit <span>Bar</span></> : <>New <span>Bar</span></>}
        </h2>
        {displayedError && <div className="bar-form__error">{displayedError}</div>}
        <div className="bar-form__field">
          <label className="bar-form__label">Name</label>
          <input className="bar-form__input" value={name} onChange={e => setName(e.target.value)} />
        </div>
        <div className="bar-form__field">
          <label className="bar-form__label">X Coord</label>
          <input className="bar-form__input" value={xCoord} onChange={e => setXCoord(e.target.value)} />
        </div>
        <div className="bar-form__field">
          <label className="bar-form__label">Y Coord</label>
          <input className="bar-form__input" value={yCoord} onChange={e => setYCoord(e.target.value)} />
        </div>
        <div className="bar-form__field">
          <label className="bar-form__label">Rating</label>
          <input className="bar-form__input" type="number" step="0.1" min="0" max="5" value={rating} onChange={e => setRating(e.target.value)} />
        </div>
        <div className="bar-form__field">
          <label className="bar-form__label">Address</label>
          <input className="bar-form__input" value={address} onChange={e => setAddress(e.target.value)} />
        </div>
        <div className="bar-form__field">
          <label className="bar-form__label">Open Time</label>
          <input className="bar-form__input" type="time" value={openTime} onChange={e => setOpenTime(e.target.value)} />
        </div>
        <div className="bar-form__field">
          <label className="bar-form__label">Close Time</label>
          <input className="bar-form__input" type="time" value={closeTime} onChange={e => setCloseTime(e.target.value)} />
        </div>
        <div className="bar-form__actions">
          <button className="btn btn--ghost" onClick={onCancel}>Cancel</button>
          <button className="btn btn--primary" onClick={submitData}>
            {initial ? 'Save Changes' : 'Add Bar'}
          </button>
        </div>
      </div>
    </div>
  );
}
