using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Framework.Mongo.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Infrastructure;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public class CashflowQueriesRepository : RepositoryRoot, ICashflowQueriesRepository
    {
        public CashflowQueriesRepository(IMongoDbContext context) : base(context)
        {
        }

        public async Task<QueryResults<CashflowViewModel>> FindAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken)
        {
            queryParams.ThrowIfNull(nameof(queryParams));

            var query =
            (
                from cs in DbContext.Cashflows.AsQueryable().Where(x => x.UserId == userId)
                join a in DbContext.Accounts.AsQueryable() on cs.Account equals a.MongoId
                join c in DbContext.Categories.AsQueryable() on cs.Category equals c.MongoId
                select new
                {
                    Id = cs.Id,
                    EffectiveDate = cs.EffectiveDate,
                    Amount = cs.Amount,
                    IntervalType = cs.IntervalType,
                    Frequency = cs.Frequency,
                    Recurrence = cs.Recurrence,
                    Description = cs.Description,
                    Tags = cs.Tags,
                    Account = a.Id,
                    AccountName = a.Name,
                    Category = c.Id,
                    CategoryName = c.Name,
                    CategoryType = c.Type,
                    CategoryColor = c.Color
                }
            ).Filter(queryParams.Filter);

            var queryResults = await query.ToListAsync(cancellationToken);
            
            var totalCount = queryResults.Count;

            var results = queryResults.AsQueryable()
                .Sort(queryParams.Sort)
                .Skip(queryParams.Offset)
                .Take(queryParams.Limit)
                .Select(cs => new CashflowViewModel
                {
                    Id = cs.Id,
                    EffectiveDate = cs.EffectiveDate,
                    Amount = cs.Amount,
                    IntervalType = cs.IntervalType,
                    Frequency = cs.Frequency,
                    Recurrence = cs.Recurrence,
                    Description = cs.Description,
                    Tags = cs.Tags.ToImmutableArray(),
                    Account = new AccountInfoViewModel(cs.Account, cs.AccountName),
                    Category = new CategoryInfoViewModel(cs.Category, cs.CategoryName, cs.CategoryType,
                        cs.CategoryColor)
                })
                .ToList();

            return new QueryResults<CashflowViewModel>(totalCount, results.ToImmutableList());
        }

        public async Task<CashflowViewModel> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            var cashflow = await
            (
                from cs in DbContext.Cashflows.AsQueryable().Where(x => x.UserId == userId)
                join a in DbContext.Accounts.AsQueryable() on cs.Account equals a.MongoId
                join c in DbContext.Categories.AsQueryable() on cs.Category equals c.MongoId
                select new
                {
                    Id = cs.Id,
                    EffectiveDate = cs.EffectiveDate,
                    Amount = cs.Amount,
                    IntervalType = cs.IntervalType,
                    Frequency = cs.Frequency,
                    Recurrence = cs.Recurrence,
                    Description = cs.Description,
                    Tags = cs.Tags,
                    Account = a.Id,
                    AccountName = a.Name,
                    Category = c.Id,
                    CategoryName = c.Name,
                    CategoryType = c.Type,
                    CategoryColor = c.Color
                }
            ).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

            return new CashflowViewModel
            {
                Id = cashflow.Id,
                EffectiveDate = cashflow.EffectiveDate,
                Amount = cashflow.Amount,
                IntervalType = cashflow.IntervalType,
                Frequency = cashflow.Frequency,
                Recurrence = cashflow.Recurrence,
                Description = cashflow.Description,
                Tags = cashflow.Tags.ToImmutableArray(),
                Account = new AccountInfoViewModel(cashflow.Account, cashflow.AccountName),
                Category = new CategoryInfoViewModel(cashflow.Category, cashflow.CategoryName, cashflow.CategoryType, cashflow.CategoryColor)
            };
        }
    }
}
