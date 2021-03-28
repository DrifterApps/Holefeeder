using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetAccountHandler : IRequestHandler<GetAccountQuery, AccountViewModel>
    {
        private readonly IAccountQueriesRepository _repository;
        private readonly ItemsCache _cache; 

        public GetAccountHandler(IAccountQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<AccountViewModel> Handle(GetAccountQuery query,
            CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            var results = await _repository.FindByIdAsync((Guid)_cache["UserId"], query.Id, cancellationToken);
            return results;
        }
    }
}
