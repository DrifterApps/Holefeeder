using System;
using System.Collections.Generic;
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
    public class TransactionQueriesRepository : RepositoryRoot, ITransactionQueriesRepository
    {
        public TransactionQueriesRepository(IMongoDbContext context) : base(context)
        {
        }

        public async Task<QueryResults<TransactionViewModel>> FindAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken)
        {
            queryParams.ThrowIfNull(nameof(queryParams));

            var query =
            (
                from t in DbContext.Transactions.AsQueryable()
                join a in DbContext.Accounts.AsQueryable() on t.Account equals a.MongoId
                join c in DbContext.Categories.AsQueryable() on t.Category equals c.MongoId
                join cs in DbContext.Cashflows.AsQueryable() on t.Cashflow equals cs.MongoId
                select new
                {
                    Id = t.Id,
                    Date = t.Date,
                    Amount = t.Amount,
                    Description = t.Description,
                    Tags = t.Tags,
                    Account = a.Id,
                    AccountName = a.Name,
                    Category = c.Id,
                    CategoryName = c.Name,
                    CategoryType = c.Type,
                    CategoryColor = c.Color,
                    Cashflow = cs.Id,
                    CashflowDate = t.CashflowDate
                }
            ).Filter(queryParams.Filter);

            var queryResults = await query.ToListAsync(cancellationToken);
            
            var totalCount = queryResults.Count;

            var results = queryResults.AsQueryable()
                .Sort(queryParams.Sort)
                .Skip(queryParams.Offset)
                .Take(queryParams.Limit)
                .Select(t => new TransactionViewModel
                {
                    Id = t.Id,
                    Date = t.Date,
                    Amount = t.Amount,
                    Description = t.Description,
                    Tags = t.Tags.ToImmutableArray(),
                    Account = new AccountInfoViewModel(t.Account, t.AccountName),
                    Category = new CategoryInfoViewModel(t.Category, t.CategoryName, t.CategoryType,
                        t.CategoryColor)
                })
                .ToList();

            return new QueryResults<TransactionViewModel>(totalCount, results.ToImmutableList());
        }

        public async Task<TransactionViewModel> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            var tx = await
            (
                from t in DbContext.Transactions.AsQueryable().Where(x => x.UserId == userId)
                join a in DbContext.Accounts.AsQueryable() on t.Account equals a.MongoId
                join c in DbContext.Categories.AsQueryable() on t.Category equals c.MongoId
                join cs in DbContext.Cashflows.AsQueryable() on t.Cashflow equals cs.MongoId
                select new
                {
                    Id = t.Id,
                    Date = t.Date,
                    Amount = t.Amount,
                    Description = t.Description,
                    Tags = t.Tags,
                    Account = a.Id,
                    AccountName = a.Name,
                    Category = c.Id,
                    CategoryName = c.Name,
                    CategoryType = c.Type,
                    CategoryColor = c.Color,
                    Cashflow = cs.Id,
                    CashflowDate = t.CashflowDate
                }
            ).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

            return new TransactionViewModel
            {
                Id = tx.Id,
                Date = tx.Date,
                Amount = tx.Amount,
                Description = tx.Description,
                Tags = tx.Tags.ToImmutableArray(),
                Account = new AccountInfoViewModel(tx.Account, tx.AccountName),
                Category = new CategoryInfoViewModel(tx.Category, tx.CategoryName, tx.CategoryType, tx.CategoryColor)
            };
        }
    }
}
