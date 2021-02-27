using System;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext
{
    public class Account : Entity, IAggregateRoot
    {
        public static Account Create(AccountType type, string name, decimal openBalance, DateTime openDate,
            string description, Guid userId)
            => new(Guid.NewGuid(), type, name, false, openBalance, openDate, description, false, userId);
        
        public Account(Guid id, AccountType type, string name, bool favorite, decimal openBalance, DateTime openDate,
            string description, bool inactive, Guid userId)
        {
            if (id.Equals(default)) throw new HolefeederDomainException($"{nameof(id)} is required");

            if (string.IsNullOrWhiteSpace(name) || name.Length > 255)
                throw new HolefeederDomainException($"{nameof(name)} must be from 1 to 255 characters");

            if (openDate.Equals(default)) throw new HolefeederDomainException($"{nameof(openDate)} is required");

            if (userId.Equals(default)) throw new HolefeederDomainException($"{nameof(userId)} is required");

            Id = id;
            Type = type;
            Name = name;
            Favorite = favorite;
            OpenBalance = openBalance;
            OpenDate = openDate;
            Description = description;
            Inactive = inactive;
            UserId = userId;
        }

        public AccountType Type { get; }
        public string Name { get; }
        public bool Favorite { get; }
        public decimal OpenBalance { get; }
        public DateTime OpenDate { get; }
        public string Description { get; }
        public bool Inactive { get; }
        public Guid UserId { get; }
    }
}
