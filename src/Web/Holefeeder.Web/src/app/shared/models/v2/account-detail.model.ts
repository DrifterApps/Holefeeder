
import { IAccountType } from '../../interfaces/v2/account-type.interface';
import { IAccountDetail } from '../../interfaces/v2/account-detail.interface';

export class AccountDetail implements IAccountDetail {
  id: string;
  name: string;
  type: IAccountType;
  transactionCount: number;
  balance: number;
  updated: Date;
  description: string;
  favorite: boolean;
}
