import { IAccountType } from './account-type.interface';
import { dateToUtc, dateFromUtc } from '../../date-parser.helper';
import { AccountDetail } from '../../models/v2/account-detail.model';

export interface IAccountDetail {
  id: string;
  name: string;
  type: IAccountType;
  transactionCount: number;
  balance: number;
  updated: Date;
  description: string;
  favorite: boolean;
}

export function accountDetailToServer(item: IAccountDetail): IAccountDetail {
  return Object.assign({} as IAccountDetail, item, {
    updated: dateToUtc(item.updated)
  });
}

export function accountDetailFromServer(item: IAccountDetail): IAccountDetail {
  return Object.assign(new AccountDetail(), item, {
      updated: dateFromUtc(item.updated)
  });
}
