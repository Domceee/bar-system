export const IngredientCategory = {
  Spirit: 0,
  Liqueur: 1,
  Wine: 2,
  Beer: 3,
  Juice: 4,
  Syrup: 5,
  Mixer: 6,
  Bitter: 7,
  Garnish: 8,
  Other: 9,
} as const;

export type IngredientCategory = typeof IngredientCategory[keyof typeof IngredientCategory];

export const IngredientCategoryLabel: Record<IngredientCategory, string> = {
  [IngredientCategory.Spirit]: 'Spirit',
  [IngredientCategory.Liqueur]: 'Liqueur',
  [IngredientCategory.Wine]: 'Wine',
  [IngredientCategory.Beer]: 'Beer',
  [IngredientCategory.Juice]: 'Juice',
  [IngredientCategory.Syrup]: 'Syrup',
  [IngredientCategory.Mixer]: 'Mixer',
  [IngredientCategory.Bitter]: 'Bitter',
  [IngredientCategory.Garnish]: 'Garnish',
  [IngredientCategory.Other]: 'Other',
};

export interface Ingredient {
  id: number;
  name: string;
  category: IngredientCategory;
  abvPercentage: number;
  price: number;
}

export interface CreateIngredientDto {
  name: string;
  category: IngredientCategory;
  abvPercentage: number;
  price: number;
}
