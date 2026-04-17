import { useEffect, useState } from 'react';
import { Pencil, Trash2 } from 'lucide-react';
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

  function addBar() {
    setFormMode({ mode: 'add' });
  }

  function pressAdd() {
    addBar();
  }

  async function editBar(id: number) {
    const bar = await fetchThisBar(id);
    setFormMode({ mode: 'edit', bar });
  }

  function pressEdit(id: number) {
    editBar(id);
  }

  async function deleteBar(id: number) {
    await deleteBarApi(id);
    openUserBars();
  }

  function pressDelete(id: number) {
    if (!window.confirm('Delete this bar?')) return;
    deleteBar(id);
  }

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
    <div className="bar-list">
      <div className="bar-list__header">
        <h1 className="bar-list__title"><span>Bars</span></h1>
        <button className="btn btn--primary" onClick={pressAdd}>+ Add Bar</button>
      </div>

      <table className="bar-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>X Coord</th>
            <th>Y Coord</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {bars.length === 0 ? (
            <tr>
              <td colSpan={4} className="bar-table__empty">No bars yet — add one to get started.</td>
            </tr>
          ) : (
            bars.map(bar => (
              <tr key={bar.id}>
                <td>{bar.name}</td>
                <td>{bar.xCoord}</td>
                <td>{bar.yCoord}</td>
                <td>
                  <div className="bar-table__actions">
                    <button className="btn btn--edit btn--icon" onClick={() => pressEdit(bar.id)}>
                      <Pencil size={15} />
                    </button>
                    <button className="btn btn--delete btn--icon" onClick={() => pressDelete(bar.id)}>
                      <Trash2 size={15} />
                    </button>
                  </div>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
