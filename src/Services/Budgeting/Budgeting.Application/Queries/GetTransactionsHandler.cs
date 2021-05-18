using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, QueryResults<TransactionViewModel>>
    {
        private readonly ITransactionQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetTransactionsHandler(ITransactionQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<QueryResults<TransactionViewModel>> Handle(GetTransactionsQuery query,
            CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            var (totalCount, transactions) = (await _repository.FindAsync((Guid)_cache["UserId"], query.Query, cancellationToken));

            return new QueryResults<TransactionViewModel>(totalCount, transactions);
        }
    }
}
