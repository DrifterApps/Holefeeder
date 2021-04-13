using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetCategoryTypesQuery : IRequest<CategoryType[]>
    {
        public GetCategoryTypesQuery()
        {
        }
    }
}
