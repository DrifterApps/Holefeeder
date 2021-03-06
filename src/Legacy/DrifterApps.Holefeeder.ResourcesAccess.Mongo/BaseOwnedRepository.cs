using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading;
using MongoDB.Bson;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public abstract class BaseOwnedRepository<TEntity, TSchema> : BaseRepository<TEntity, TSchema>, IBaseOwnedRepository<TEntity>
        where TEntity : BaseEntity, IOwnedEntity
        where TSchema : BaseSchema, IOwnedSchema
    {
        protected BaseOwnedRepository(IMongoCollection<TSchema> collection, IMapper mapper) : base(collection, mapper)
        {
        }

        public async Task<bool> IsOwnerAsync(Guid userId, string id, CancellationToken cancellationToken = default)
        {
            if (ObjectId.TryParse(id, out _))
            {
                return await Collection.AsQueryable().AnyAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
            }
            if (Guid.TryParse(id, out var globalId))
            {
                return await Collection.AsQueryable().AnyAsync(x => x.GlobalId == globalId && x.UserId == userId, cancellationToken);
            }

            return false;
        }

        public Task<int> CountAsync(Guid userId, QueryParams query, CancellationToken cancellationToken = default) => Collection.AsQueryable().Where(x => x.UserId == userId).Filter(query?.Filter).CountAsync(cancellationToken);

        public async Task<IEnumerable<TEntity>> FindAsync(Guid userId, QueryParams queryParams, CancellationToken cancellationToken = default)
        {
            var query = Collection.AsQueryable().Where(x => x.UserId == userId).Filter(queryParams?.Filter).Sort(queryParams?.Sort).Offset(queryParams?.Offset).Limit(queryParams?.Limit);
            return Mapper.Map<IEnumerable<TEntity>>(await query.ToListAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}
