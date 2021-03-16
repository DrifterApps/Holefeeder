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
    public class GetAccountsHandler : IRequestHandler<GetAccountsQuery, AccountViewModel[]>
    {
        private readonly IAccountQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetAccountsHandler(IAccountQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<AccountViewModel[]> Handle(GetAccountsQuery query,
            CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            return (await _repository.FindAsync((Guid)_cache["UserId"], query.Query, cancellationToken)).ToArray();
        }
    }
}
