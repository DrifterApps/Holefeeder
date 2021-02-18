using System;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders;
using DrifterApps.Holefeeder.Budgeting.Infrastructure;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;
using DrifterApps.Holefeeder.Framework.SeedWork;

using MongoDB.Bson;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests
{
    public static class BudgetingContextSeed
    {
        public static readonly Guid TestUserGuid1 = Guid.NewGuid();
        public static readonly Guid TestUserGuid2 = Guid.NewGuid();

        public static readonly (Guid Id, ObjectId MongoId) Account1 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Account2 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Account3 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Account4 = new(Guid.NewGuid(), ObjectId.GenerateNewId());

        public static readonly (Guid Id, ObjectId MongoId) Category1 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Category2 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Category3 = new(Guid.NewGuid(), ObjectId.GenerateNewId());

        public static readonly (Guid Id, ObjectId MongoId) Cashflow1 = new(Guid.NewGuid(), ObjectId.GenerateNewId());

        public static readonly (Guid Id, ObjectId MongoId) Transaction1 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Transaction2 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Transaction3 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Transaction4 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Transaction5 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Transaction6 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Transaction7 = new(Guid.NewGuid(), ObjectId.GenerateNewId());
        public static readonly (Guid Id, ObjectId MongoId) Transaction8 = new(Guid.NewGuid(), ObjectId.GenerateNewId());

        public static void SeedData(IHolefeederDatabaseSettings settings)
        {
            settings.ThrowIfNull(nameof(settings));

            var client = new MongoClient(settings.ConnectionString);

            CleanupData(client, settings);

            var database = client.GetDatabase(settings.Database);

            var accounts = database.GetCollection<AccountSchema>(AccountSchema.SCHEMA);
            AccountBuilder.Create(Account1).OfType(AccountType.Checking).ForUser(TestUserGuid1).Build();
            AccountBuilder.Create(Account2).OfType(AccountType.CreditCard).ForUser(TestUserGuid1).IsFavorite().Build();
            AccountBuilder.Create(Account3).OfType(AccountType.Loan).ForUser(TestUserGuid1).IsInactive().Build();
            AccountBuilder.Create(Account4).OfType(AccountType.Checking).ForUser(TestUserGuid2).Build();
            accounts.InsertMany(AccountBuilder.Accounts);

            var categories = database.GetCollection<CategorySchema>(CategorySchema.SCHEMA);
            CategoryBuilder.Create(Category1).OfType(CategoryType.Expense).ForUser(TestUserGuid1).Build();
            CategoryBuilder.Create(Category2).OfType(CategoryType.Gain).ForUser(TestUserGuid1).IsFavorite().Build();
            CategoryBuilder.Create(Category3).OfType(CategoryType.Expense).ForUser(TestUserGuid2).Build();
            categories.InsertMany(CategoryBuilder.Categories);

            var cashflows = database.GetCollection<CashflowSchema>(CashflowSchema.SCHEMA);
            CashflowBuilder.Create(Cashflow1).OfType(DateIntervalType.Weekly).WithFrequency(2).OfAmount(111)
                .ForAccount(Account1).ForCategory(Category1).ForUser(TestUserGuid1).Build();
            cashflows.InsertMany(CashflowBuilder.Cashflows);

            var transactions = database.GetCollection<TransactionSchema>(TransactionSchema.SCHEMA);
            TransactionBuilder.Create(Transaction1).OfAmount(10).ForAccount(Account1).ForCategory(Category1)
                .ForUser(TestUserGuid1).Build();
            TransactionBuilder.Create(Transaction2).OfAmount(20).ForAccount(Account1).ForCategory(Category1)
                .ForUser(TestUserGuid1).Build();
            TransactionBuilder.Create(Transaction3).OfAmount(30).ForAccount(Account1).ForCategory(Category1)
                .ForUser(TestUserGuid1).Build();
            TransactionBuilder.Create(Transaction4).OfAmount(40).ForAccount(Account1).ForCategory(Category2)
                .ForUser(TestUserGuid1).Build();
            TransactionBuilder.Create(Transaction5).OfAmount(50).ForAccount(Account1).ForCategory(Category2)
                .ForUser(TestUserGuid1).Build();
            TransactionBuilder.Create(Transaction6).OfAmount(111).IsCashflow(Cashflow1, new DateTime(2020, 1, 2))
                .ForAccount(Account1).ForCategory(Category1).ForUser(TestUserGuid1).Build();
            TransactionBuilder.Create(Transaction7).OfAmount(10).ForAccount(Account4).ForCategory(Category3)
                .ForUser(TestUserGuid2).Build();
            TransactionBuilder.Create(Transaction8).OfAmount(20).ForAccount(Account4).ForCategory(Category3)
                .ForUser(TestUserGuid2).Build();
            transactions.InsertMany(TransactionBuilder.Transactions);
        }

        private static void CleanupData(IMongoClient client, IHolefeederDatabaseSettings settings)
        {
            var databases = client.ListDatabaseNames().ToList();
            if (databases.Any(db => settings.Database.Equals(db, StringComparison.OrdinalIgnoreCase)))
            {
                client.DropDatabase(settings.Database);
            }
        }
    }
}
