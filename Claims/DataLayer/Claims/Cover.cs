using Claims.Core;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.DataLayer.Claims;

public class Cover
{
    [BsonId]
    public string Id { get; set; }

    [BsonElement("startDate")]
    //[BsonDateTimeOptions(DateOnly = true)]
    public DateOnly StartDate { get; set; }

    [BsonElement("endDate")]
    //[BsonDateTimeOptions(DateOnly = true)]
    public DateOnly EndDate { get; set; }

    [BsonElement("claimType")]
    public CoverType Type { get; set; }

    [BsonElement("premium")]
    public decimal Premium { get; set; }
}