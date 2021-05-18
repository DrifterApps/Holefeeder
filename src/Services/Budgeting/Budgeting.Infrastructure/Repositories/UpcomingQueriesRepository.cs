using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public class UpcomingQueriesRepository : RepositoryRoot, IUpcomingQueriesRepository
    {
        public UpcomingQueriesRepository(IMongoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(Guid userId, DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            var transactionCollection = DbContext.Transactions;
            var cashflowCollection = DbContext.Cashflows;
            var accountCollection = DbContext.Accounts;
            var categoryCollection = DbContext.Categories;

            var pastCashflows = await transactionCollection.AsQueryable()
                .Where(x => x.UserId == userId && !string.IsNullOrEmpty(x.Cashflow))
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.Cashflow)
                .Select(g => new
                {
                    Cashflow = g.Key, LastPaidDate = g.First().Date, LastCashflowDate = g.First().CashflowDate
                }).ToListAsync(cancellationToken);

            var cashflows = await cashflowCollection
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
            var accounts = await accountCollection
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
            var categories = await categoryCollection
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

            var upcomingCashflows = cashflows
                .Where(x => !x.Inactive)
                .Join(accounts,
                    c => c.Account,
                    a => a.MongoId,
                    (c, a) => new {Cashflow = c, Account = a})
                .Join(categories,
                    j => j.Cashflow.Category,
                    c => c.MongoId,
                    (j, c) => new {j.Cashflow, j.Account, Category = c});

            var results =
                (from c in upcomingCashflows
                    join t in pastCashflows on c.Cashflow.MongoId equals t.Cashflow into u
                    from t in u.DefaultIfEmpty()
                    select new
                    {
                        c.Cashflow,
                        c.Account,
                        c.Category,
                        t?.LastPaidDate,
                        t?.LastCashflowDate
                    })
                .SelectMany(x =>
                {
                    var dates = new List<DateTime>();

                    dates.AddRange(x.Cashflow.IntervalType
                        .DatesInRange(x.Cashflow.EffectiveDate, startDate, endDate, x.Cashflow.Frequency)
                        .Where(futureDate =>
                            IsUnpaid(x.Cashflow.EffectiveDate, futureDate, x.LastPaidDate, x.LastCashflowDate)));

                    var date = x.Cashflow.IntervalType.PreviousDate(x.Cashflow.EffectiveDate, startDate,
                        x.Cashflow.Frequency);
                    while (IsUnpaid(x.Cashflow.EffectiveDate, date, x.LastPaidDate, x.LastCashflowDate) &&
                           date > x.Cashflow.EffectiveDate)
                    {
                        dates.Add(date);
                        date = x.Cashflow.IntervalType.PreviousDate(x.Cashflow.EffectiveDate, date,
                            x.Cashflow.Frequency);
                    }

                    return dates.Select(d =>
                        new UpcomingViewModel
                        {
                            Id = x.Cashflow.Id,
                            Date = d,
                            Amount = x.Cashflow.Amount,
                            Description = x.Cashflow.Description,
                            Tags = x.Cashflow.Tags?.ToImmutableArray() ?? ImmutableArray<string>.Empty,
                            Category = new CategoryInfoViewModel(x.Category.Id, x.Category.Name, x.Category.Type,
                                x.Category.Color),
                            Account = new AccountInfoViewModel(x.Account.Id, x.Account.Name)
                        });
                }).Where(x => x.Date <= endDate)
                .OrderBy(x => x.Date);

            return results;
        }

        private static bool IsUnpaid(DateTime effectiveDate, DateTime nextDate, DateTime? lastPaidDate,
            DateTime? lastCashflowDate) =>
            !lastPaidDate.HasValue
                ? (nextDate >= effectiveDate)
                : (nextDate > lastPaidDate && nextDate > lastCashflowDate);
    }
}
