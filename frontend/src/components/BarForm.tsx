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
    return (
      <div>
        <h2>{initial ? 'Edit Bar' : 'Add Bar'}</h2>
        {(validationError || error) && (
          <p style={{ color: 'red' }}>{validationError ?? error}</p>
        )}
        <div>
          <label>Name</label>
          <input value={name} onChange={e => setName(e.target.value)} />
        </div>
        <div>
          <label>X Coord</label>
          <input value={xCoord} onChange={e => setXCoord(e.target.value)} />
        </div>
        <div>
          <label>Y Coord</label>
          <input value={yCoord} onChange={e => setYCoord(e.target.value)} />
        </div>
        <button onClick={submitForm}>{initial ? 'Save' : 'Add'}</button>
        <button onClick={onCancel}>Cancel</button>
      </div>
    );
  }

  return displayForm();
}
