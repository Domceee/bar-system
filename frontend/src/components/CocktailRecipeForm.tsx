import { useEffect, useMemo, useState } from 'react';
import { Trash2 } from 'lucide-react';
import type { CocktailRecipe, CreateCocktailRecipeDto, RecipeIngredientInput } from '../types/cocktailRecipe';
import { IngredientUnit, IngredientUnitLabel } from '../types/cocktailRecipe';
import type { Ingredient } from '../types/ingredient';
import { IngredientCategoryLabel } from '../types/ingredient';
import { fetchIngredients } from '../api/ingredientApi';
import { previewRecipe } from '../api/cocktailRecipeApi';

interface Props {
  initial?: CocktailRecipe;
  onSubmit: (data: CreateCocktailRecipeDto) => Promise<void>;
  onCancel: () => void;
  error: string | null;
}

export default function CocktailRecipeForm({ initial, onSubmit, onCancel, error }: Props) {
  const [name, setName] = useState(initial?.name ?? '');
  const [description, setDescription] = useState(initial?.description ?? '');
  const [servings, setServings] = useState(initial?.servings?.toString() ?? '1');
  const [items, setItems] = useState<RecipeIngredientInput[]>(
    initial?.ingredients.map(i => ({ ingredientId: i.ingredientId, quantity: i.quantity, unit: i.unit })) ?? []
  );

  const [allIngredients, setAllIngredients] = useState<Ingredient[]>([]);
  const [search, setSearch] = useState('');
  const [selectedNewId, setSelectedNewId] = useState<string>('');
  const [newQty, setNewQty] = useState<string>('30');
  const [newUnit, setNewUnit] = useState<IngredientUnit>(IngredientUnit.Ml);

  const [preview, setPreview] = useState<{ abv: number; cost: number }>({ abv: initial?.abv ?? 0, cost: 0 });
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => { fetchIngredients().then(setAllIngredients); }, []);

  const filteredOptions = useMemo(() => {
    const s = search.trim().toLowerCase();
    return s ? allIngredients.filter(i => i.name.toLowerCase().includes(s)) : allIngredients;
  }, [allIngredients, search]);

  const ingMap = useMemo(() => new Map(allIngredients.map(i => [i.id, i])), [allIngredients]);

  useEffect(() => {
    if (items.length === 0) return;
    let cancelled = false;
    previewRecipe(Number(servings) || 1, items)
      .then(res => { if (!cancelled) setPreview({ abv: res.abv, cost: res.cost }); })
      .catch(() => { /* ignore preview errors */ });
    return () => { cancelled = true; };
  }, [items, servings]);

  const displayedAbv = items.length === 0 ? 0 : preview.abv;
  const displayedCost = items.length === 0 ? 0 : preview.cost;

  function addItem() {
    setValidationError(null);
    const id = Number(selectedNewId);
    const qty = Number(newQty);
    if (!id) { setValidationError('Pick an ingredient first.'); return; }
    if (!qty || qty <= 0) { setValidationError('Quantity must be greater than zero.'); return; }
    if (items.some(i => i.ingredientId === id)) { setValidationError('Ingredient already in the recipe.'); return; }
    setItems(prev => [...prev, { ingredientId: id, quantity: qty, unit: newUnit }]);
    setSelectedNewId('');
    setNewQty('30');
  }

  function updateItem(idx: number, patch: Partial<RecipeIngredientInput>) {
    setItems(prev => prev.map((it, i) => i === idx ? { ...it, ...patch } : it));
  }

  function removeItem(idx: number) {
    setItems(prev => prev.filter((_, i) => i !== idx));
  }

  function isDataValid(): boolean {
    if (!name.trim()) { setValidationError('Name is required.'); return false; }
    const s = Number(servings);
    if (!s || s <= 0) { setValidationError('Servings must be greater than zero.'); return false; }
    if (items.length === 0) { setValidationError('Add at least one ingredient.'); return false; }
    if (items.some(i => !i.quantity || i.quantity <= 0)) { setValidationError('All quantities must be positive.'); return false; }
    setValidationError(null);
    return true;
  }

  async function submitData() {
    if (!isDataValid()) return;
    await onSubmit({
      name: name.trim(),
      description: description.trim(),
      servings: Number(servings),
      authorId: initial?.authorId ?? null,
      ingredients: items,
    });
  }

  const displayedError = validationError ?? error;

  return (
    <div className="bar-form-page">
      <div className="bar-form" style={{ maxWidth: 720 }}>
        <h2 className="bar-form__title">
          {initial ? <>Edit <span>Recipe</span></> : <>New <span>Recipe</span></>}
        </h2>
        {displayedError && <div className="bar-form__error">{displayedError}</div>}

        <div className="bar-form__field">
          <label className="bar-form__label">Name</label>
          <input className="bar-form__input" value={name} onChange={e => setName(e.target.value)} />
        </div>

        <div className="bar-form__field">
          <label className="bar-form__label">Description</label>
          <textarea className="bar-form__input" rows={3} value={description} onChange={e => setDescription(e.target.value)} />
        </div>

        <div className="bar-form__field">
          <label className="bar-form__label">Servings</label>
          <input className="bar-form__input" type="number" min="1" value={servings} onChange={e => setServings(e.target.value)} />
        </div>

        <div className="bar-form__field">
          <label className="bar-form__label">Search ingredients</label>
          <input className="bar-form__input" value={search} onChange={e => setSearch(e.target.value)} placeholder="Type to filter…" />
        </div>

        <div className="bar-form__field">
          <label className="bar-form__label">Add ingredient</label>
          <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
            <select className="bar-form__input" style={{ flex: 2, minWidth: 200 }} value={selectedNewId} onChange={e => setSelectedNewId(e.target.value)}>
              <option value="">— choose —</option>
              {filteredOptions.map(opt => (
                <option key={opt.id} value={opt.id}>
                  {opt.name} ({IngredientCategoryLabel[opt.category]}, {opt.abvPercentage}% ABV)
                </option>
              ))}
            </select>
            <input className="bar-form__input" style={{ flex: 1, minWidth: 80 }} type="number" min="0" step="0.1" value={newQty} onChange={e => setNewQty(e.target.value)} />
            <select className="bar-form__input" style={{ flex: 1, minWidth: 80 }} value={newUnit} onChange={e => setNewUnit(Number(e.target.value) as IngredientUnit)}>
              {Object.entries(IngredientUnitLabel).map(([k, label]) => (
                <option key={k} value={k}>{label}</option>
              ))}
            </select>
            <button className="btn btn--primary" onClick={addItem}>Add</button>
          </div>
        </div>

        <div className="bar-form__field">
          <label className="bar-form__label">Ingredients ({items.length})</label>
          {items.length === 0 ? (
            <div style={{ opacity: 0.7 }}>No ingredients yet.</div>
          ) : (
            <table className="bar-table">
              <thead>
                <tr><th>Ingredient</th><th>ABV%</th><th>Qty</th><th>Unit</th><th></th></tr>
              </thead>
              <tbody>
                {items.map((it, idx) => {
                  const ing = ingMap.get(it.ingredientId);
                  return (
                    <tr key={`${it.ingredientId}-${idx}`}>
                      <td>{ing?.name ?? `#${it.ingredientId}`}</td>
                      <td>{ing?.abvPercentage ?? 0}%</td>
                      <td>
                        <input className="bar-form__input" type="number" min="0" step="0.1" value={it.quantity}
                          onChange={e => updateItem(idx, { quantity: Number(e.target.value) })} />
                      </td>
                      <td>
                        <select className="bar-form__input" value={it.unit}
                          onChange={e => updateItem(idx, { unit: Number(e.target.value) as IngredientUnit })}>
                          {Object.entries(IngredientUnitLabel).map(([k, label]) => (
                            <option key={k} value={k}>{label}</option>
                          ))}
                        </select>
                      </td>
                      <td>
                        <button className="btn btn--delete btn--icon" onClick={() => removeItem(idx)}><Trash2 size={15} /></button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          )}
        </div>

        <div className="bar-form__field">
          <label className="bar-form__label">Live preview</label>
          <div>ABV: <strong>{displayedAbv.toFixed(2)}%</strong> &nbsp;·&nbsp; Estimated cost: <strong>{displayedCost.toFixed(2)}</strong></div>
        </div>

        <div className="bar-form__actions">
          <button className="btn btn--ghost" onClick={onCancel}>Cancel</button>
          <button className="btn btn--primary" onClick={submitData}>
            {initial ? 'Save Changes' : 'Create Recipe'}
          </button>
        </div>
      </div>
    </div>
  );
}
