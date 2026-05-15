import type {
  AbvPreviewResponse,
  CocktailRecipe,
  CreateCocktailRecipeDto,
  OptimizeRequest,
  OptimizeResponse,
  PublishResult,
  RecipeIngredientInput,
  UpdateCocktailRecipeDto,
} from '../types/cocktailRecipe';

const BASE = 'http://localhost:5029/api/cocktailrecipe';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    try {
      const parsed = JSON.parse(text);
      throw new Error(parsed.error ?? text);
    } catch {
      throw new Error(text || res.statusText);
    }
  }
  return res.json() as Promise<T>;
}

export const fetchRecipes = (): Promise<CocktailRecipe[]> =>
  fetch(BASE).then(handleResponse<CocktailRecipe[]>);

export const fetchRecipe = (id: number): Promise<CocktailRecipe> =>
  fetch(`${BASE}/${id}`).then(handleResponse<CocktailRecipe>);

export const createRecipe = (dto: CreateCocktailRecipeDto): Promise<CocktailRecipe> =>
  fetch(BASE, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  }).then(handleResponse<CocktailRecipe>);

export const updateRecipe = (id: number, dto: UpdateCocktailRecipeDto): Promise<CocktailRecipe> =>
  fetch(`${BASE}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  }).then(handleResponse<CocktailRecipe>);

export const deleteRecipe = async (id: number): Promise<void> => {
  const res = await fetch(`${BASE}/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error(await res.text() || res.statusText);
};

export const publishRecipe = (id: number): Promise<PublishResult> =>
  fetch(`${BASE}/${id}/publish`, { method: 'POST' }).then(handleResponse<PublishResult>);

export const confirmPublishRecipe = (id: number, mode: 'remix' | 'original'): Promise<CocktailRecipe> =>
  fetch(`${BASE}/${id}/publish/confirm`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ mode }),
  }).then(handleResponse<CocktailRecipe>);

export const previewRecipe = (servings: number, ingredients: RecipeIngredientInput[]): Promise<AbvPreviewResponse> =>
  fetch(`${BASE}/preview`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ servings, ingredients }),
  }).then(handleResponse<AbvPreviewResponse>);

export const optimizeRecipe = (id: number, req: OptimizeRequest): Promise<OptimizeResponse> =>
  fetch(`${BASE}/${id}/optimize`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(req),
  }).then(handleResponse<OptimizeResponse>);
