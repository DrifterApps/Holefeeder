using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;

using MongoDB.Bson;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders
{
    public class CategoryBuilder
    {
        private static readonly object _locker = new();
        private static int _seed = 1;
        private static readonly List<CategorySchema> _categories = new();

        public static IReadOnlyList<CategorySchema> Categories => _categories;

        private ObjectId _mongoId;
        private Guid _id;
        private string _name;
        private CategoryType _type;
        private bool _favorite;
        private Guid _userId;

        public static CategoryBuilder Create((Guid Id, ObjectId MongoId) id)
        {
            lock (_locker)
            {
                var (guid, mongoId) = id;
                return new CategoryBuilder(guid, mongoId, _seed);
            }
        }

        private CategoryBuilder(Guid id, ObjectId mongoId, int seed)
        {
            _mongoId = mongoId;
            _id = id;
            _name = $"Category{seed}";
            _type = CategoryType.Expense;
            _favorite = false;
            _userId = Guid.NewGuid();
        }

        public CategoryBuilder OfType(CategoryType type)
        {
            _type = type;

            return this;
        }

        public CategoryBuilder IsFavorite()
        {
            _favorite = true;

            return this;
        }

        public CategoryBuilder ForUser(Guid userId)
        {
            _userId = userId;

            return this;
        }

        public void Build()
        {
            var schema = new CategorySchema()
            {
                MongoId = _mongoId.ToString(),
                Id = _id,
                Name = _name,
                Type = _type,
                UserId = _userId,
                Favorite = _favorite
            };

            lock (_locker)
            {
                _seed++;
                _categories.Add(schema);
            }
        }
    }
}
