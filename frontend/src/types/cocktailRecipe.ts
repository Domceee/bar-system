import type { IngredientCategory } from './ingredient';

export const RecipeStatus = {
  Draft: 0,
  Published: 1,
} as const;

export type RecipeStatus = typeof RecipeStatus[keyof typeof RecipeStatus];

export const IngredientUnit = {
  Ml: 0,
  Cl: 1,
  Oz: 2,
  Dash: 3,
  Piece: 4,
  Tsp: 5,
  Tbsp: 6,
} as const;

export type IngredientUnit = typeof IngredientUnit[keyof typeof IngredientUnit];

export const IngredientUnitLabel: Record<IngredientUnit, string> = {
  [IngredientUnit.Ml]: 'ml',
  [IngredientUnit.Cl]: 'cl',
  [IngredientUnit.Oz]: 'oz',
  [IngredientUnit.Dash]: 'dash',
  [IngredientUnit.Piece]: 'pc',
  [IngredientUnit.Tsp]: 'tsp',
  [IngredientUnit.Tbsp]: 'tbsp',
};

export interface RecipeIngredientInput {
  ingredientId: number;
  quantity: number;
  unit: IngredientUnit;
}

export interface RecipeIngredient {
  id: number;
  ingredientId: number;
  ingredientName: string;
  category: IngredientCategory;
  abvPercentage: number;
  price: number;
  quantity: number;
  unit: IngredientUnit;
}

export interface CocktailRecipe {
  id: number;
  name: string;
  description: string;
  status: RecipeStatus;
  isPublished: boolean;
  abv: number;
  servings: number;
  createdAt: string;
  authorId: number | null;
  ingredients: RecipeIngredient[];
}

export interface CreateCocktailRecipeDto {
  name: string;
  description: string;
  servings: number;
  authorId: number | null;
  ingredients: RecipeIngredientInput[];
}

export interface UpdateCocktailRecipeDto {
  name: string;
  description: string;
  servings: number;
  ingredients: RecipeIngredientInput[];
}

export interface PublishResult {
  status: 'published' | 'duplicate' | 'notFound';
  duplicateOfId: number | null;
  duplicateOfName: string | null;
  similarity: number | null;
}

export interface AbvPreviewResponse {
  abv: number;
  cost: number;
}

export type OptimizeMode = 'stronger' | 'weaker' | 'cheaper' | 'changeType';

export interface OptimizeRequest {
  mode: OptimizeMode;
  targetAbv: number | null;
  preferCategory: IngredientCategory | null;
}

export interface OptimizeResponse {
  ingredients: RecipeIngredientInput[];
  estimatedAbv: number;
  estimatedCost: number;
  notes: string;
}
