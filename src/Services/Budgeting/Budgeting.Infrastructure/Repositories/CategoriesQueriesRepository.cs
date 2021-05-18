﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public class CategoriesQueriesRepository : RepositoryRoot, ICategoryQueries
    {
        private readonly IMapper _mapper;

        public CategoriesQueriesRepository(IMongoDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var categoryCollection = DbContext.Categories;

            var categories = await categoryCollection.AsQueryable()
                .Where(x => !x.System && x.UserId == userId).ToListAsync(cancellationToken: cancellationToken);

            return _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
        }
    }
}
