using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.ResourcesAccess
{
    public interface IBaseOwnedRepository<T> : IBaseRepository<T> where T : BaseEntity, IOwnedEntity
    {
        Task<bool> IsOwnerAsync(Guid userId, string id, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Guid userId, QueryParams query, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(Guid userId, QueryParams queryParams, CancellationToken cancellationToken = default);
    }
}
