using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MongoDB.Models
{
  public class Post
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public User Author { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public List<Comment> Comments { get; set; }
    public List<User> Upvotes { get; set; }
    public List<User> Downvotes { get; set; }
  }
}
