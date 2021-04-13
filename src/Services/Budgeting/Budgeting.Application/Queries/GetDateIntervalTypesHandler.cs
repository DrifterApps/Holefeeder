using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetDateIntervalTypesHandler : IRequestHandler<GetDateIntervalTypesQuery, DateIntervalType[]>
    {
        public GetDateIntervalTypesHandler()
        {
        }

        public Task<DateIntervalType[]> Handle(GetDateIntervalTypesQuery query, CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            return Task.FromResult(Enumeration.GetAll<DateIntervalType>().ToArray());
        }
    }
}
