import { NgModule } from '@angular/core';
import { Routes, RouterModule, ExtraOptions } from '@angular/router';
import { ErrorNotfoundComponent } from './error-notfound/error-notfound.component';
import { AuthGuardService } from './auth/services/auth-guard.service';

const appRoutes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule), canActivate: [AuthGuardService]
  },
  {
    path: 'accounts',
    loadChildren: () => import('./accounts/accounts.module').then(m => m.AccountsModule), canActivate: [AuthGuardService]
  },
  {
    path: 'settings',
    loadChildren: () => import('./settings/settings.module').then(m => m.SettingsModule), canActivate: [AuthGuardService]
  },
  {
    path: 'cashflows',
    loadChildren: () => import('./cashflows/cashflows.module').then(m => m.CashflowsModule), canActivate: [AuthGuardService]
  },
  {
    path: 'transactions',
    loadChildren: () => import('./transactions/transactions.module').then(m => m.TransactionsModule), canActivate: [AuthGuardService]
  },
  {
    path: 'statistics',
    loadChildren: () => import('./statistics/statistics.module').then(m => m.StatisticsModule), canActivate: [AuthGuardService]
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
  },
  { path: 'auth', loadChildren: () => import('./auth/auth.module').then(x => x.AuthModule) },
  { path: 'oauthcallback', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', component: ErrorNotfoundComponent }
];

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }