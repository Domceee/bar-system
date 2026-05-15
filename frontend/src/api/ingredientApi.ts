import type { CreateIngredientDto, Ingredient } from '../types/ingredient';

const BASE = 'http://localhost:5029/api/ingredient';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json() as Promise<T>;
}

export const fetchIngredients = (search?: string): Promise<Ingredient[]> => {
  const url = search && search.trim() ? `${BASE}?search=${encodeURIComponent(search.trim())}` : BASE;
  return fetch(url).then(handleResponse<Ingredient[]>);
};

export const createIngredient = (dto: CreateIngredientDto): Promise<Ingredient> =>
  fetch(BASE, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  }).then(handleResponse<Ingredient>);
