using System;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public record GetTransactionQuery : IRequest<TransactionViewModel>
    {
        public Guid Id { get; init; }
    }
}