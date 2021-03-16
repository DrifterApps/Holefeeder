using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;
using DrifterApps.Holefeeder.Framework.Mongo.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public IUnitOfWork UnitOfWork { get; }
        private readonly IMongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;

        public AccountRepository(IUnitOfWork unitOfWork, IMongoDbContext mongoDbContext, IMapper mapper)
        {
            UnitOfWork = unitOfWork.ThrowIfNull(nameof(mapper));
            _mongoDbContext = mongoDbContext.ThrowIfNull(nameof(mongoDbContext));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        public async Task<Account> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var collection = await _mongoDbContext.GetAccountsAsync(cancellationToken);

            var schema = await collection.AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

            var cashflowsCollection = await _mongoDbContext.GetCashflowsAsync(cancellationToken);

            var schemaId = schema.MongoId;
            var cashflows = await cashflowsCollection.AsQueryable()
                .Where(x => x.Account == schemaId).Select(x => x.Id).ToListAsync(cancellationToken);

            var entity = new Account
            {
                Id = schema.Id,
                Type=schema.Type,
                Name=schema.Name,
                Favorite = schema.Favorite,
                OpenBalance = schema.OpenBalance,
                OpenDate = schema.OpenDate,
                Description = schema.Description,
                Inactive = schema.Inactive,
                UserId = schema.UserId,
                Cashflows = cashflows
            };
            return entity;
        }

        public async Task<Account> FindByNameAsync(string name, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var collection = await _mongoDbContext.GetAccountsAsync(cancellationToken);

            var schema = await collection.AsQueryable()
                .FirstOrDefaultAsync(
                    x => x.Name.ToLowerInvariant() == name.ToLowerInvariant() && x.UserId == userId,
                    cancellationToken);

            return _mapper.Map<Account>(schema);
        }

        public async Task SaveAsync(Account entity, CancellationToken cancellationToken = default)
        {
            var collection = await _mongoDbContext.GetAccountsAsync(cancellationToken);

            var id = entity.Id;
            var userId = entity.UserId;

            var schema = await collection.AsQueryable().SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId,
                cancellationToken: cancellationToken);

            schema = schema == null ? _mapper.Map<AccountSchema>(entity) : _mapper.Map(entity, schema);

            _mongoDbContext.AddCommand(async () =>
            {
                await collection.ReplaceOneAsync(x => x.Id == schema.Id,
                    schema,
                    new ReplaceOptions {IsUpsert = true},
                    cancellationToken);
            });
        }
    }
}
