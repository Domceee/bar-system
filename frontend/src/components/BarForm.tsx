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
  const [validationError, setValidationError] = useState<string | null>(null);

  function isDataValid(): boolean {
    if (!name.trim()) {
      setValidationError('Name is required.');
      return false;
    }
    if (isNaN(Number(xCoord)) || isNaN(Number(yCoord))) {
      setValidationError('Coordinates must be valid numbers.');
      return false;
    }
    setValidationError(null);
    return true;
  }

  async function submitData() {
    if (!isDataValid()) return;
    await onSubmit({ name: name.trim(), xCoord: Number(xCoord), yCoord: Number(yCoord) });
  }

  function submitForm() {
    submitData();
  }

  function displayForm() {
    const displayedError = validationError ?? error;

    return (
      <div className="bar-form-page">
        <div className="bar-form">
          <h2 className="bar-form__title">
            {initial ? <>Edit <span>Bar</span></> : <>New <span>Bar</span></>}
          </h2>

          {displayedError && (
            <div className="bar-form__error">{displayedError}</div>
          )}

          <div className="bar-form__field">
            <label className="bar-form__label">Name</label>
            <input
              className="bar-form__input"
              placeholder="e.g. The Purple Lounge"
              value={name}
              onChange={e => setName(e.target.value)}
            />
          </div>

          <div className="bar-form__field">
            <label className="bar-form__label">X Coord</label>
            <input
              className="bar-form__input"
              placeholder="e.g. 54.6872"
              value={xCoord}
              onChange={e => setXCoord(e.target.value)}
            />
          </div>

          <div className="bar-form__field">
            <label className="bar-form__label">Y Coord</label>
            <input
              className="bar-form__input"
              placeholder="e.g. 25.2797"
              value={yCoord}
              onChange={e => setYCoord(e.target.value)}
            />
          </div>

          <div className="bar-form__actions">
            <button className="btn btn--ghost" onClick={onCancel}>Cancel</button>
            <button className="btn btn--primary" onClick={submitForm}>
              {initial ? 'Save Changes' : 'Add Bar'}
            </button>
          </div>
        </div>
      </div>
    );
  }

  return displayForm();
}
