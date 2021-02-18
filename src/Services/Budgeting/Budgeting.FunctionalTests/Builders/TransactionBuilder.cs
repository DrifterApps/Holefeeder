using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;

using MongoDB.Bson;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders
{
    public class TransactionBuilder
    {
        private static readonly object _locker = new();
        private static int _seed = 1;
        private static readonly List<TransactionSchema> _transactions = new();

        public static IReadOnlyList<TransactionSchema> Transactions => _transactions;

        private ObjectId _mongoId;
        private Guid _id;
        private string _description;
        private DateTime _date;
        private decimal _amount;
        private (Guid Id, ObjectId MongoId) _account;
        private (Guid Id, ObjectId MongoId) _category;
        private (Guid Id, ObjectId MongoId) _cashflow;
        private Guid _userId;
        private DateTime? _cashflowDate;

        public static TransactionBuilder Create((Guid Id, ObjectId MongoId) id)
        {
            lock (_locker)
            {
                var (guid, mongoId) = id;
                return new TransactionBuilder(guid, mongoId, _seed);
            }
        }

        private TransactionBuilder(Guid id, ObjectId mongoId, int seed)
        {
            _mongoId = mongoId;
            _id = id;
            _amount = (Convert.ToDecimal(seed) * 100m) + (Convert.ToDecimal(seed) / 100m);
            _date = (new DateTime(2020, 1, 1)).AddDays(seed);
            _description = $"Transaction{seed}";
            _account = (Guid.Empty, ObjectId.Empty);
            _category = (Guid.Empty, ObjectId.Empty);
            _userId = Guid.Empty;
            _cashflow = (Guid.Empty, ObjectId.Empty);
            _cashflowDate = null;
        }

        public TransactionBuilder OfAmount(decimal amount)
        {
            _amount = amount;

            return this;
        }

        public TransactionBuilder On(DateTime date)
        {
            _date = date;

            return this;
        }

        public TransactionBuilder ForAccount((Guid Id, ObjectId MongoId) account)
        {
            _account = account;

            return this;
        }

        public TransactionBuilder ForCategory((Guid Id, ObjectId MongoId) category)
        {
            _category = category;

            return this;
        }

        public TransactionBuilder IsCashflow((Guid Id, ObjectId MongoId) cashflow, DateTime cashflowDate)
        {
            _cashflow = cashflow;
            _cashflowDate = cashflowDate;

            return this;
        }

        public TransactionBuilder ForUser(Guid userId)
        {
            _userId = userId;

            return this;
        }

        public void Build()
        {
            var schema = new TransactionSchema()
            {
                MongoId = _mongoId.ToString(),
                Id = _id,
                Amount = _amount,
                Date = _date,
                Description = _description,
                UserId = _userId,
                Account = _account.MongoId.ToString(),
                Category = _category.MongoId.ToString(),
                Cashflow = _cashflow.Id == Guid.Empty ? null : _cashflow.MongoId.ToString(),
                CashflowDate = _cashflowDate
            };

            lock (_locker)
            {
                _seed++;
                _transactions.Add(schema);
            }
        }
    }
}
