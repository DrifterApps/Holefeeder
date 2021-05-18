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
    public class GetCashflowsHandler : IRequestHandler<GetCashflowsQuery, QueryResults<CashflowViewModel>>
    {
        private readonly ICashflowQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetCashflowsHandler(ICashflowQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<QueryResults<CashflowViewModel>> Handle(GetCashflowsQuery query,
            CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            var (totalCount, cashflows) = (await _repository.FindAsync((Guid)_cache["UserId"], query.Query, cancellationToken));

            return new QueryResults<CashflowViewModel>(totalCount, cashflows);
        }
    }
}
