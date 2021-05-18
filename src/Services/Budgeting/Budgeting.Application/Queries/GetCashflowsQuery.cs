using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetCashflowsQuery : IRequest<QueryResults<CashflowViewModel>>
    {
        public QueryParams Query { get; }

        public GetCashflowsQuery(int? offset, int? limit, IEnumerable<string> sort,
            IEnumerable<string> filter)
        {
            Query = new QueryParams(offset ?? QueryParams.DefaultOffset, limit ?? QueryParams.DefaultLimit,
                sort ?? QueryParams.DefaultSort, filter ?? QueryParams.DefaultFilter);
        }
    }
}
