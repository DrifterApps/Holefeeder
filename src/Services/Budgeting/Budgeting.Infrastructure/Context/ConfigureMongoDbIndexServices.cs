using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Context
{
    public class ConfigureMongoDbIndexesService : IHostedService
    {
        private readonly IMongoDbContext _context;
        private readonly ILogger<ConfigureMongoDbIndexesService> _logger;

        public ConfigureMongoDbIndexesService(IMongoDbContext context, ILogger<ConfigureMongoDbIndexesService> logger)
            => (_context, _logger) = (context, logger);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await CreateAccountsIndexes(cancellationToken);
            await CreateCashflowsIndexes(cancellationToken);
            await CreateCategoriesIndexes(cancellationToken);
            await CreateTransactionsIndexes(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        private async Task CreateAccountsIndexes(CancellationToken cancellationToken)
        {
            var collection = _context.Accounts;

            _logger.LogInformation($"Creating 'Id' Index on {AccountSchema.SCHEMA}");
            var keys = Builders<AccountSchema>.IndexKeys.Ascending(a => a.Id);
            var options = new CreateIndexOptions {Unique = true};
            await collection.Indexes.CreateOneAsync(new CreateIndexModel<AccountSchema>(keys, options), cancellationToken: cancellationToken);
        }

        private async Task CreateTransactionsIndexes(CancellationToken cancellationToken)
        {
            var collection = _context.Transactions;

            _logger.LogInformation($"Creating 'Id' Index on {TransactionSchema.SCHEMA}");
            var idKeys = Builders<TransactionSchema>.IndexKeys.Ascending(a => a.Id);
            var idOptions = new CreateIndexOptions {Unique = true};
            await collection.Indexes.CreateOneAsync(new CreateIndexModel<TransactionSchema>(idKeys, idOptions), cancellationToken: cancellationToken);

            _logger.LogInformation($"Creating 'Account' Index on {TransactionSchema.SCHEMA}");
            var accountKeys = Builders<TransactionSchema>.IndexKeys
                .Ascending(a => a.Account);
            await collection.Indexes.CreateOneAsync(new CreateIndexModel<TransactionSchema>(accountKeys), cancellationToken: cancellationToken);

            _logger.LogInformation($"Creating 'Date' Index on {TransactionSchema.SCHEMA}");
            var dateKeys = Builders<TransactionSchema>.IndexKeys
                .Descending(s => s.Date);
            await collection.Indexes.CreateOneAsync(new CreateIndexModel<TransactionSchema>(dateKeys), cancellationToken: cancellationToken);
        }

        private async Task CreateCashflowsIndexes(CancellationToken cancellationToken)
        {
            var collection = _context.Cashflows;

            _logger.LogInformation($"Creating 'Id' Index on {CashflowSchema.SCHEMA}");
            var keys = Builders<CashflowSchema>.IndexKeys.Ascending(a => a.Id);
            var options = new CreateIndexOptions {Unique = true};
            await collection.Indexes.CreateOneAsync(new CreateIndexModel<CashflowSchema>(keys, options), cancellationToken: cancellationToken);
        }

        private async Task CreateCategoriesIndexes(CancellationToken cancellationToken)
        {
            var collection = _context.Categories;

            _logger.LogInformation($"Creating 'Id' Index on {CategorySchema.SCHEMA}");
            var keys = Builders<CategorySchema>.IndexKeys.Ascending(a => a.Id);
            var options = new CreateIndexOptions {Unique = true};
            await collection.Indexes.CreateOneAsync(new CreateIndexModel<CategorySchema>(keys, options), cancellationToken: cancellationToken);
        }
    }
}
