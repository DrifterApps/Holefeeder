using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    [BsonIgnoreExtraElements]
    public class ObjectDataSchema : BaseSchema, IOwnedSchema
    {
        [BsonElement("code")]
        public string Code { get; set; }

        [BsonElement("data")]
        public string Data { get; set; }

        [BsonElement("userId")]
        public Guid UserId { get; set; }
    }
}
