﻿using System;
using DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Context;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Schemas;
using MongoDB.Bson;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Tests
{
    public static class GenerateTestData
    {
        public static (ObjectId MongoId, Guid Id) CreateTestUserSchema(this MongoDbContext context, string testName)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var mongoId = ObjectId.GenerateNewId();
            var id = Guid.NewGuid();

            context.GetUsersAsync().Result.InsertOne(new UserSchema
            {
                MongoId = mongoId.ToString(),
                FirstName = "TestUser",
                LastName = testName,
                EmailAddress = $"{id.ToString()}@email.com",
                DateJoined = DateTime.Today,
                GoogleId = string.Empty,
                Id = id
            });
            return (mongoId, id);
        }

        public static (ObjectId MongoId, Guid Id) CreateAccountSchema(this MongoDbContext context, int index,
            AccountType type, (ObjectId MongoId, Guid Id) user, bool favorite = false, bool inactive = false)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (index > 99 || index < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, @"Value must be between 1 and 99");
            }

            var mongoId = ObjectId.GenerateNewId();
            var id = Guid.NewGuid();

            context.GetAccountsAsync().Result.InsertOne(new AccountSchema
            {
                MongoId = mongoId.ToString(),
                Name = $"Account{index}",
                Type = type,
                Description = $"Account #{index}",
                Id = id,
                Favorite = favorite,
                Inactive = inactive,
                OpenBalance = (Convert.ToDecimal(index) * 100m) + (Convert.ToDecimal(index) / 100m),
                OpenDate = (new DateTime(2019, 1, 1)).AddDays(index),
                UserId = user.MongoId.ToString()
            });
            return (mongoId, id);
        }

        public static (ObjectId MongoId, Guid Id) CreateCategorySchema(this MongoDbContext context, int index,
            CategoryType type, (ObjectId MongoId, Guid Id) user, bool favorite = false)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (index > 99 || index < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, @"Value must be between 1 and 99");
            }

            var mongoId = ObjectId.GenerateNewId();
            var id = Guid.NewGuid();

            context.GetCategoriesAsync().Result.InsertOne(new CategorySchema
            {
                MongoId = mongoId.ToString(),
                Name = $"Category{index}",
                Type = type,
                Id = id,
                Favorite = favorite,
                UserId = user.MongoId.ToString()
            });

            return (mongoId, id);
        }

        public static (ObjectId MongoId, Guid Id) CreateTransactionSchema(this MongoDbContext context, int index,
            (ObjectId MongoId, Guid Id) account, (ObjectId MongoId, Guid Id) category, decimal amount,
            (ObjectId MongoId, Guid Id) user)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (index > 99 || index < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, @"Value must be between 1 and 99");
            }

            var mongoId = ObjectId.GenerateNewId();
            var id = Guid.NewGuid();

            context.GetTransactionsAsync().Result.InsertOne(new TransactionSchema
            {
                MongoId = mongoId.ToString(),
                Amount = amount,
                Date = (new DateTime(2020, 1, 1)).AddDays(index),
                Description = $"Transaction{index}",
                Account = account.MongoId.ToString(),
                Category = category.MongoId.ToString(),
                Id = id,
                UserId = user.MongoId.ToString()
            });

            return (mongoId, id);
        }
    }
}
