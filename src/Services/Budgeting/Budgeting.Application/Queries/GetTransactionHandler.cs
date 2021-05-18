using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetTransactionHandler : IRequestHandler<GetTransactionQuery, TransactionViewModel>
    {
        private readonly ITransactionQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetTransactionHandler(ITransactionQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<TransactionViewModel> Handle(GetTransactionQuery query, CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            var transaction = (await _repository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken));

            return transaction;
        }
    }
}
