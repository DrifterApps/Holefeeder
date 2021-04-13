import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {AccountsServiceV2} from '@app/shared/services/accounts-v2.service';
import {IAccountDetail} from '@app/shared/interfaces/v2/account-detail.interface';
import {UpcomingService} from '@app/singletons/services/upcoming.service';
import {Subject} from 'rxjs';
import {faPlus} from '@fortawesome/free-solid-svg-icons';
import {IUpcoming} from "@app/shared/interfaces/v2/upcoming.interface";
import { categoryTypeMultiplier } from '@app/shared/interfaces/v2/category-type.interface';
import { accountTypeMultiplier } from '@app/shared/interfaces/v2/account-type.interface';
import { AccountTypeNames } from '@app/shared/enums/account-type.enum';

@Component({
  templateUrl: './accounts-list.component.html',
  styleUrls: ['./accounts-list.component.scss']
})
export class AccountsListComponent implements OnInit {
  accounts: IAccountDetail[];
  upcomingCashflows: IUpcoming[];
  accountTypeNames: Map<string, string>;
  showInactive = false;
  $showInactive = new Subject<boolean>();

  faPlus = faPlus;

  constructor(
    private accountService: AccountsServiceV2,
    private upcomingService: UpcomingService,
    private router: Router
  ) {
    this.accountTypeNames = AccountTypeNames;
  }

  ngOnInit() {
    this.$showInactive.subscribe(async (showInactive) => {
      this.accounts = await this.accountService.find(null, null, [
        '-favorite',
        'name'
      ], [
        showInactive ? 'inactive=true' : 'inactive!=true'
      ]);
    });
    this.upcomingService.cashflows.subscribe(cashflows => this.upcomingCashflows = cashflows);
    this.$showInactive.next(this.showInactive);
  }

  click(id: string) {
    this.router.navigate(['accounts', id]);
  }

  inactiveChange() {
    this.showInactive = !this.showInactive;
    this.$showInactive.next(this.showInactive);
  }

  getUpcomingBalance(account: IAccountDetail): number {
    console.log(account);
    return (
      account.balance +
      (this.upcomingCashflows ?
        this.upcomingCashflows
          .filter(cashflow => cashflow.account.id === account.id)
          .map(
            cashflow => {
              console.log(cashflow);
              return cashflow.amount *
              categoryTypeMultiplier(cashflow.category.type) *
              accountTypeMultiplier(account.type)
            }
          )
          .reduce((sum, current) => sum + current, 0) : 0)
    );
  }

  amountClass(account: IAccountDetail): string {
    const val = account.balance * accountTypeMultiplier(account.type);
    if (val < 0) {
      return 'text-danger';
    } else if (val > 0) {
      return 'text-success';
    } else {
      return 'text-info';
    }
  }
}
