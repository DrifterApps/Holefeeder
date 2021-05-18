using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Queries;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

namespace DrifterApps.Holefeeder.Budgeting.Application.Contracts
{
    public interface ITransactionQueriesRepository
    {
        Task<QueryResults<TransactionViewModel>> FindAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken = default);

        Task<TransactionViewModel> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    }
}
