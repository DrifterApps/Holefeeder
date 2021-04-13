using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetAccountTypesHandler : IRequestHandler<GetAccountTypesQuery, AccountType[]>
    {
        public GetAccountTypesHandler()
        {
        }

        public Task<AccountType[]> Handle(GetAccountTypesQuery query, CancellationToken cancellationToken = default)
        {
            query.ThrowIfNull(nameof(query));

            return Task.FromResult(Enumeration.GetAll<AccountType>().ToArray());
        }
    }
}
