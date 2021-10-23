using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Models
{
  public class Report
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string ReportedLink { get; set; }
    public User ReportingUser { get; set; }
    public User ReportedUser { get; set; }
    public string Description { get; set; }
  }
}
