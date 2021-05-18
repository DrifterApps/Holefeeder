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
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, CategoryViewModel[]>
    {
        private readonly ICategoryQueries _categoryQueries;
        private readonly ItemsCache _cache; 

        public GetCategoriesHandler(ICategoryQueries categoryQueries, ItemsCache cache)
        {
            _categoryQueries = categoryQueries;
            _cache = cache;
        }

        public async Task<CategoryViewModel[]> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            return (await _categoryQueries.GetCategoriesAsync((Guid)_cache["UserId"], cancellationToken)).ToArray();
        }
    }
}
