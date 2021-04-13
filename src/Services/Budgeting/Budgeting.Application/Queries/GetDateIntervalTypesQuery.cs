using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetDateIntervalTypesQuery : IRequest<DateIntervalType[]>
    {
        public GetDateIntervalTypesQuery()
        {
        }
    }
}
