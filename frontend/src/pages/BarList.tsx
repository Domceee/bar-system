import { useEffect, useState } from 'react';
import { fetchBars, fetchThisBar, createBar, updateBar, deleteBar as deleteBarApi } from '../api/barApi';
import type { Bar, CreateBarDto } from '../types/bar';
import BarForm from '../components/BarForm';

type FormMode = { mode: 'add' } | { mode: 'edit'; bar: Bar } | null;

export default function BarList() {
  const [bars, setBars] = useState<Bar[]>([]);
  const [formMode, setFormMode] = useState<FormMode>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);

  function displayBars(data: Bar[]) {
    setBars(data);
  }

  function openUserBars() {
    fetchBars().then(displayBars);
  }

  useEffect(() => {
    openUserBars();
  }, []);

  // --- Add ---
  function addBar() {
    setFormMode({ mode: 'add' });
  }

  function pressAdd() {
    addBar();
  }

  // --- Edit ---
  async function editBar(id: number) {
    const bar = await fetchThisBar(id);
    setFormMode({ mode: 'edit', bar });
  }

  function pressEdit(id: number) {
    editBar(id);
  }

  // --- Delete ---
  async function deleteBar(id: number) {
    await deleteBarApi(id);
    openUserBars();
  }

  function pressDelete(id: number) {
    if (!window.confirm('Delete this bar?')) return;
    deleteBar(id);
  }

  // --- Form submit (called by BarForm after submitData) ---
  async function handleFormSubmit(data: CreateBarDto) {
    try {
      setSubmitError(null);
      if (formMode?.mode === 'edit') {
        await updateBar(formMode.bar.id, data);
      } else {
        await createBar(data);
      }
      setFormMode(null);
      openUserBars();
    } catch (e: unknown) {
      setSubmitError(e instanceof Error ? e.message : 'An error occurred.');
    }
  }

  if (formMode) {
    return (
      <BarForm
        initial={formMode.mode === 'edit' ? formMode.bar : undefined}
        onSubmit={handleFormSubmit}
        onCancel={() => setFormMode(null)}
        error={submitError}
      />
    );
  }

  return (
    <div>
      <h1>Bars</h1>
      <button onClick={pressAdd}>Add Bar</button>
      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th>X</th>
            <th>Y</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {bars.map(bar => (
            <tr key={bar.id}>
              <td>{bar.name}</td>
              <td>{bar.xCoord}</td>
              <td>{bar.yCoord}</td>
              <td>
                <button onClick={() => pressEdit(bar.id)}>Edit</button>
                <button onClick={() => pressDelete(bar.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
