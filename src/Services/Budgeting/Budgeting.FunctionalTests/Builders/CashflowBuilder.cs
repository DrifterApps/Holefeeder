using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;

using MongoDB.Bson;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders
{
    public class CashflowBuilder
    {
        private static readonly object _locker = new();
        private static int _seed = 1;
        private static readonly List<CashflowSchema> _cashflows = new();

        public static IReadOnlyList<CashflowSchema> Cashflows => _cashflows;

        private ObjectId _mongoId;
        private Guid _id;
        private string _description;
        private decimal _amount;
        private DateTime _effectiveDate;
        private DateIntervalType _type;
        private int _frequency;
        private int _recurrence;
        private (Guid Id, ObjectId MongoId) _account;
        private (Guid Id, ObjectId MongoId) _category;
        private bool _inactive;
        private Guid _userId;

        public static CashflowBuilder Create((Guid Id, ObjectId MongoId) id)
        {
            lock (_locker)
            {
                var (guid, mongoId) = id;
                return new CashflowBuilder(guid, mongoId, _seed);
            }
        }

        private CashflowBuilder(Guid id, ObjectId mongoId, int seed)
        {
            _mongoId = mongoId;
            _id = id;
            _amount = (Convert.ToDecimal(seed) * 100m) + (Convert.ToDecimal(seed) / 100m);
            _description = $"Cashflow{seed}";
            _effectiveDate = (new DateTime(2020, 1, 1)).AddDays(seed);
            _type = DateIntervalType.Weekly;
            _frequency = 2;
            _recurrence = 0;
            _account = (Guid.Empty, ObjectId.Empty);
            _category = (Guid.Empty, ObjectId.Empty);
            _inactive = false;
            _userId = Guid.Empty;
        }
        
        public CashflowBuilder OfAmount(decimal amount)
        {
            _amount = amount;

            return this;
        }
        
        public CashflowBuilder OfType(DateIntervalType type)
        {
            _type = type;

            return this;
        }
        
        public CashflowBuilder WithFrequency(int frequency)
        {
            _frequency = frequency;

            return this;
        }
        
        public CashflowBuilder ForAccount((Guid Id, ObjectId MongoId) account)
        {
            _account = account;

            return this;
        }
        
        public CashflowBuilder ForCategory((Guid Id, ObjectId MongoId) category)
        {
            _category = category;

            return this;
        }

        public CashflowBuilder IsInactive()
        {
            _inactive = true;

            return this;
        }

        public CashflowBuilder ForUser(Guid userId)
        {
            _userId = userId;

            return this;
        }

        public void Build()
        {
            var schema = new CashflowSchema()
            {
                MongoId = _mongoId.ToString(),
                Id = _id,
                Amount = _amount,
                EffectiveDate = _effectiveDate,
                Description = _description,
                IntervalType = _type,
                Frequency = _frequency,
                Recurrence = _recurrence,
                UserId = _userId,
                Inactive = _inactive,
                Account = _account.MongoId.ToString(),
                Category = _category.MongoId.ToString()
            };

            lock (_locker)
            {
                _seed++;
                _cashflows.Add(schema);
            }
        }
    }
}
