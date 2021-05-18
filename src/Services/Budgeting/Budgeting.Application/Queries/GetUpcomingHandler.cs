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
    public class GetUpcomingHandler : IRequestHandler<GetUpcomingQuery, UpcomingViewModel[]>
    {
        private readonly IUpcomingQueriesRepository _repository;
        private readonly ItemsCache _cache;

        public GetUpcomingHandler(IUpcomingQueriesRepository repository, ItemsCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<UpcomingViewModel[]> Handle(GetUpcomingQuery query,
            CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            var results = await _repository.GetUpcomingAsync((Guid)_cache["UserId"], query.From, query.To, cancellationToken);
            return results.ToArray();
        }
    }
}
