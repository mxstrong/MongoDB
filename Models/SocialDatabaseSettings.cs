namespace MongoDB.Models
{
  public class SocialDatabaseSettings: ISocialDatabaseSettings
  {
    public string PostsCollectionName { get; set; }
    public string UsersCollectionName { get; set; }
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
  }

  public interface ISocialDatabaseSettings
  {
    string PostsCollectionName { get; set; }
    string UsersCollectionName { get; set; }
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
  }
}
