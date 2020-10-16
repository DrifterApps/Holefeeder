using DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Schemas
{
    [BsonIgnoreExtraElements]
    public class CategorySchema : BaseSchema, IOwnedSchema
    {
        public const string SCHEMA = "categories";
        
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("type")]
        public CategoryType Type { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [BsonElement("budgetAmount"), BsonRepresentation(BsonType.Decimal128)]
        public decimal BudgetAmount { get; set; }

        [BsonElement("favorite")]
        public bool Favorite { get; set; }

        [BsonElement("system")]
        public bool System { get; set; }

        [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
    }
}