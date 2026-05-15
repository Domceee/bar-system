import { useEffect, useState } from 'react';
import { Pencil, Trash2 } from 'lucide-react';
import {
  fetchRecipes,
  fetchRecipe,
  createRecipe,
  updateRecipe,
  deleteRecipe as deleteRecipeApi,
  publishRecipe,
  confirmPublishRecipe,
  optimizeRecipe,
} from '../api/cocktailRecipeApi';
import type {
  CocktailRecipe,
  CreateCocktailRecipeDto,
  OptimizeMode,
  PublishResult,
} from '../types/cocktailRecipe';
import { RecipeStatus } from '../types/cocktailRecipe';
import CocktailRecipeForm from '../components/CocktailRecipeForm';

type FormMode =
  | { mode: 'add' }
  | { mode: 'edit'; recipe: CocktailRecipe }
  | null;

export default function CocktailRecipeList() {
  const [recipes, setRecipes] = useState<CocktailRecipe[]>([]);
  const [formMode, setFormMode] = useState<FormMode>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [duplicatePrompt, setDuplicatePrompt] = useState<{ recipeId: number; result: PublishResult } | null>(null);
  const [optimizeFor, setOptimizeFor] = useState<{ recipe: CocktailRecipe; notes: string; previewAbv: number; previewCost: number } | null>(null);

  function displayRecipes(data: CocktailRecipe[]) { setRecipes(data); }
  function reloadRecipes() { fetchRecipes().then(displayRecipes); }

  useEffect(() => { reloadRecipes(); }, []);

  function pressAdd() { setFormMode({ mode: 'add' }); }

  async function pressEdit(id: number) {
    const r = await fetchRecipe(id);
    setFormMode({ mode: 'edit', recipe: r });
  }

  function pressDelete(id: number) {
    if (!window.confirm('Delete this recipe?')) return;
    deleteRecipeApi(id).then(reloadRecipes);
  }

  async function handleFormSubmit(data: CreateCocktailRecipeDto) {
    try {
      setSubmitError(null);
      if (formMode?.mode === 'edit') await updateRecipe(formMode.recipe.id, data);
      else await createRecipe(data);
      setFormMode(null);
      reloadRecipes();
    } catch (e: unknown) {
      setSubmitError(e instanceof Error ? e.message : 'An error occurred.');
    }
  }

  async function pressPublish(id: number) {
    const result = await publishRecipe(id);
    if (result.status === 'duplicate') {
      setDuplicatePrompt({ recipeId: id, result });
    } else {
      reloadRecipes();
    }
  }

  async function resolveDuplicate(mode: 'remix' | 'original') {
    if (!duplicatePrompt) return;
    await confirmPublishRecipe(duplicatePrompt.recipeId, mode);
    setDuplicatePrompt(null);
    reloadRecipes();
  }

  async function pressOptimize(recipe: CocktailRecipe, mode: OptimizeMode) {
    const res = await optimizeRecipe(recipe.id, { mode, targetAbv: null, preferCategory: null });
    setOptimizeFor({ recipe, notes: res.notes, previewAbv: res.estimatedAbv, previewCost: res.estimatedCost });
  }

  async function applyOptimization() {
    if (!optimizeFor) return;
    // re-pull the proposed ingredients via the same optimize endpoint, then save as update
    const res = await optimizeRecipe(optimizeFor.recipe.id, { mode: 'cheaper', targetAbv: null, preferCategory: null });
    await updateRecipe(optimizeFor.recipe.id, {
      name: optimizeFor.recipe.name,
      description: optimizeFor.recipe.description,
      servings: optimizeFor.recipe.servings,
      ingredients: res.ingredients,
    });
    setOptimizeFor(null);
    reloadRecipes();
  }

  if (formMode) {
    return (
      <CocktailRecipeForm
        initial={formMode.mode === 'edit' ? formMode.recipe : undefined}
        onSubmit={handleFormSubmit}
        onCancel={() => { setFormMode(null); setSubmitError(null); }}
        error={submitError}
      />
    );
  }

  return (
    <div className="bar-list">
      <div className="bar-list__header">
        <h1 className="bar-list__title"><span>Cocktail Recipes</span></h1>
        <button className="btn btn--primary" onClick={pressAdd}>+ New Recipe</button>
      </div>

      {duplicatePrompt && (
        <div className="bar-form__error" style={{ marginBottom: 12 }}>
          Near-duplicate detected: <strong>{duplicatePrompt.result.duplicateOfName}</strong>
          {duplicatePrompt.result.similarity != null && (
            <> ({Math.round(duplicatePrompt.result.similarity * 100)}% similar)</>
          )}
          <div style={{ display: 'flex', gap: 8, marginTop: 8 }}>
            <button className="btn btn--primary" onClick={() => resolveDuplicate('remix')}>Publish as Remix</button>
            <button className="btn btn--primary" onClick={() => resolveDuplicate('original')}>Publish as Original</button>
            <button className="btn btn--ghost" onClick={() => setDuplicatePrompt(null)}>Cancel</button>
          </div>
        </div>
      )}

      {optimizeFor && (
        <div className="bar-form__error" style={{ marginBottom: 12 }}>
          Optimized <strong>{optimizeFor.recipe.name}</strong>: {optimizeFor.notes}
          <div style={{ marginTop: 4 }}>Projected ABV: {optimizeFor.previewAbv.toFixed(2)}% · Cost: {optimizeFor.previewCost.toFixed(2)}</div>
          <div style={{ display: 'flex', gap: 8, marginTop: 8 }}>
            <button className="btn btn--primary" onClick={applyOptimization}>Apply</button>
            <button className="btn btn--ghost" onClick={() => setOptimizeFor(null)}>Dismiss</button>
          </div>
        </div>
      )}

      <table className="bar-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Status</th>
            <th>ABV</th>
            <th>Servings</th>
            <th>Ingredients</th>
            <th>Created</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {recipes.length === 0 ? (
            <tr><td colSpan={7} className="bar-table__empty">No recipes yet — create one to get started.</td></tr>
          ) : (
            recipes.map(r => (
              <tr key={r.id}>
                <td>{r.name}</td>
                <td>{r.status === RecipeStatus.Published ? 'Published' : 'Draft'}</td>
                <td>{r.abv.toFixed(2)}%</td>
                <td>{r.servings}</td>
                <td>{r.ingredients.length}</td>
                <td>{new Date(r.createdAt).toLocaleDateString()}</td>
                <td>
                  <div className="bar-table__actions" style={{ flexWrap: 'wrap' }}>
                    <button className="btn btn--edit btn--icon" onClick={() => pressEdit(r.id)}><Pencil size={15} /></button>
                    <button className="btn btn--delete btn--icon" onClick={() => pressDelete(r.id)}><Trash2 size={15} /></button>
                    {!r.isPublished && (
                      <button className="btn btn--primary" onClick={() => pressPublish(r.id)}>Publish</button>
                    )}
                    <button className="btn btn--ghost" onClick={() => pressOptimize(r, 'stronger')}>Stronger</button>
                    <button className="btn btn--ghost" onClick={() => pressOptimize(r, 'weaker')}>Weaker</button>
                    <button className="btn btn--ghost" onClick={() => pressOptimize(r, 'cheaper')}>Cheaper</button>
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
