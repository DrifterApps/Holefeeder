import { Injectable } from "@angular/core";
import { IAccountDetail } from "@app/shared/interfaces/v2/account-detail.interface";
import { BehaviorSubject } from "rxjs";

@Injectable()
export class AccountService {

  // Observable IAccountDetail sources
  private accountSelectedSource = new BehaviorSubject<IAccountDetail>(null);

  // Observable IAccountDetail streams
  accountSelected$ = this.accountSelectedSource.asObservable();

  // Service message commands
  accountSelected(account: IAccountDetail) {
    this.accountSelectedSource.next(account);
  }
}
