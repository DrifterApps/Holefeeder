export interface ICategoryType {
  id: number;
  name: string;
}

export function categoryTypeMultiplier(type: ICategoryType): number {
  return type.name === 'Expense' ? -1 : 1;
}
