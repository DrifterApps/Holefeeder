using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Serializers;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Context
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        private readonly IMongoClient _mongoClient;
        private readonly List<Func<Task>> _commands;

        private IClientSessionHandle _session;

        static MongoDbContext()
        {
#pragma warning disable 618
            MongoDefaults.GuidRepresentation = GuidRepresentation.Standard;
#pragma warning restore 618
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BsonSerializer.RegisterSerializer(typeof(AccountType), new AccountTypeSerializer());
            BsonSerializer.RegisterSerializer(typeof(CategoryType), new CategoryTypeSerializer());
            BsonSerializer.RegisterSerializer(typeof(DateIntervalType), new DateIntervalTypeSerializer());
            BsonClassMap.RegisterClassMap<AccountSchema>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<CategorySchema>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<TransactionSchema>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

        public MongoDbContext(IHolefeederDatabaseSettings settings)
        {
            settings.ThrowIfNull(nameof(settings));
            
            _mongoClient = new MongoClient(settings.ConnectionString);
            _database = _mongoClient.GetDatabase(settings.Database);

            _commands = new List<Func<Task>>();
        }

        public async Task<IMongoCollection<AccountSchema>> GetAccountsAsync(
            CancellationToken cancellationToken = default)
        {
            var collection = _database.GetCollection<AccountSchema>(AccountSchema.SCHEMA);

            var indexes = await collection.Indexes.ListAsync(cancellationToken);
            var hasIndexes = await indexes.AnyAsync(cancellationToken);
            if (hasIndexes)
            {
                return collection;
            }

            var indexBuilder = Builders<AccountSchema>.IndexKeys;
            var keys = indexBuilder.Ascending(a => a.Id);
            var options = new CreateIndexOptions {Background = true, Unique = true};
            var indexModel = new CreateIndexModel<AccountSchema>(keys, options);
            await collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);

            return collection;
        }

        public async Task<IMongoCollection<CategorySchema>> GetCategoriesAsync(
            CancellationToken cancellationToken = default)
        {
            var collection = _database.GetCollection<CategorySchema>(CategorySchema.SCHEMA);

            var indexes = await collection.Indexes.ListAsync(cancellationToken);
            var hasIndexes = await indexes.AnyAsync(cancellationToken);
            if (hasIndexes)
            {
                return collection;
            }

            var indexBuilder = Builders<CategorySchema>.IndexKeys;
            var keys = indexBuilder.Ascending(c => c.Id);
            var options = new CreateIndexOptions {Background = true, Unique = true};
            var indexModel = new CreateIndexModel<CategorySchema>(keys, options);
            await collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);

            return collection;
        }

        public async Task<IMongoCollection<CashflowSchema>> GetCashflowsAsync(
            CancellationToken cancellationToken = default)
        {
            var collection = _database.GetCollection<CashflowSchema>(CashflowSchema.SCHEMA);

            var indexes = await collection.Indexes.ListAsync(cancellationToken);
            var hasIndexes = await indexes.AnyAsync(cancellationToken);
            if (hasIndexes)
            {
                return collection;
            }

            var indexBuilder = Builders<CashflowSchema>.IndexKeys;
            var keys = indexBuilder.Ascending(c => c.Id);
            var options = new CreateIndexOptions {Background = true, Unique = true};
            var indexModel = new CreateIndexModel<CashflowSchema>(keys, options);
            await collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);

            return collection;
        }

        public async Task<IMongoCollection<TransactionSchema>> GetTransactionsAsync(
            CancellationToken cancellationToken = default)
        {
            var collection = _database.GetCollection<TransactionSchema>(TransactionSchema.SCHEMA);

            var indexes = await collection.Indexes.ListAsync(cancellationToken);
            var hasIndexes = await indexes.AnyAsync(cancellationToken);
            if (hasIndexes)
            {
                return collection;
            }

            var indexBuilder = Builders<TransactionSchema>.IndexKeys;
            var keys = indexBuilder.Ascending(t => t.Id);
            var options = new CreateIndexOptions {Background = true, Unique = true};
            var indexModel = new CreateIndexModel<TransactionSchema>(keys, options);
            await collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);

            return collection;
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            using (_session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken))
            {
                _session.StartTransaction();

                var commandTasks = _commands.Select(c => c()).ToList();

                await Task.WhenAll(commandTasks);

                commandTasks.Clear();

                await _session.CommitTransactionAsync(cancellationToken);

                return commandTasks.Count;
            }
        }

        public void ClearCommands()
        {
            _commands.Clear();
        }

        private bool _isDisposed;
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                ClearCommands();
                _session?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
