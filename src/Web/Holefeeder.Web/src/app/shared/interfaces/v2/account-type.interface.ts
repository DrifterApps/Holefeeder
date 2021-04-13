export interface IAccountType {
  id: number;
  name: string;
}

export function accountTypeMultiplier(type: IAccountType): number {
  switch (type.name) {
    case 'Checking':
    case 'Investment':
    case 'Savings':
      return 1;
    default:
      return -1;
  }
}
