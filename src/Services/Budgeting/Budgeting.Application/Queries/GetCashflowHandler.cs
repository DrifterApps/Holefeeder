using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetCashflowHandler : IRequestHandler<GetCashflowQuery, CashflowViewModel>
    {
        private readonly ICashflowQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetCashflowHandler(ICashflowQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<CashflowViewModel> Handle(GetCashflowQuery query, CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            var result = (await _repository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken));

            return result;
        }
    }
}
