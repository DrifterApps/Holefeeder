using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Schemas;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Context
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        private readonly IMongoClient _mongoClient;
        private readonly List<Func<Task>> _commands;

        private IClientSessionHandle _session;

        static MongoDbContext()
        {
            BsonSerializer.RegisterSerializer(typeof(AccountType), new AccountTypeSerializer());
            BsonSerializer.RegisterSerializer(typeof(CategoryType), new CategoryTypeSerializer());
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

        public MongoDbContext(IMongoClient mongoClient, IMongoDatabase database)
        {
            _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            
            _commands = new List<Func<Task>>();
        }
        
        public async Task<IMongoCollection<AccountSchema>> GetAccountsAsync(CancellationToken cancellationToken = default)
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
            var options = new CreateIndexOptions
            {
                Background = true,
                Unique = true
            };
            var indexModel = new CreateIndexModel<AccountSchema>(keys, options);
            await collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);

            return collection;
        }

        public async Task<IMongoCollection<CategorySchema>> GetCategoriesAsync(CancellationToken cancellationToken = default)
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
            var options = new CreateIndexOptions
            {
                Background = true,
                Unique = true
            };
            var indexModel = new CreateIndexModel<CategorySchema>(keys, options);
            await collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);

            return collection;
        }
        
        public async Task<IMongoCollection<TransactionSchema>> GetTransactionsAsync(CancellationToken cancellationToken = default)
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
            var options = new CreateIndexOptions
            {
                Background = true,
                Unique = true
            };
            var indexModel = new CreateIndexModel<TransactionSchema>(keys, options);
            await collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);

            return collection;
        }
        
        public async Task<IMongoCollection<UserSchema>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            var collection = _database.GetCollection<UserSchema>(UserSchema.SCHEMA);

            var indexes = await collection.Indexes.ListAsync(cancellationToken);
            var hasIndexes = await indexes.AnyAsync(cancellationToken);
            if (hasIndexes)
            {
                return collection;
            }

            var indexBuilder = Builders<UserSchema>.IndexKeys;
            var keys = indexBuilder.Ascending(t => t.Id);
            var options = new CreateIndexOptions
            {
                Background = true,
                Unique = true
            };
            var indexModel = new CreateIndexModel<UserSchema>(keys, options);
            await collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
            
            keys = indexBuilder.Ascending(t => t.EmailAddress);
            indexModel = new CreateIndexModel<UserSchema>(keys, options);
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

        public void Dispose()
        {
            _session?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
