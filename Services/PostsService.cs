using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.Services
{
  public class PostsService
  {
    private readonly IMongoCollection<Post> _posts;

    public PostsService(IMongoCollection<Post> posts)
    {
      _posts = posts;
    }

    public async Task<List<Post>> GetAllPosts()
    {
      var posts = await _posts.FindAsync(post => true);
      return await posts.ToListAsync();
    }

    public async Task<Post> GetPostById(string id)
    {
      var post = await _posts.FindAsync<Post>(post => post.Id == id);
      return await post.FirstOrDefaultAsync();
    }
      
    public async Task<Post> CreatePost(Post post)
    {
      await _posts.InsertOneAsync(post);
      return post;
    }

    public async Task<Post> AddComment(string id, Comment comment) 
    {
      comment.Id = ObjectId.GenerateNewId().ToString();
      var update = Builders<Post>.Update
        .Push<Comment>(p => p.Comments, comment);

      return await _posts.FindOneAndUpdateAsync(post => post.Id == id, update);
    }

    public async Task<Post> RemoveComment(string postId, string commentId)
    {
      var update = Builders<Post>.Update
        .PullFilter<Comment>(p => p.Comments, c => c.Id == commentId);
      return await _posts.FindOneAndUpdateAsync(post => post.Id == postId, update);
    }

    public async Task<Post> UpvotePost(string id, User user)
    {
      var removeDownvote = Builders<Post>.Update
        .PullFilter<User>(p => p.Downvotes, u => u.Id == user.Id);
      var upvote = Builders<Post>.Update
        .AddToSet<User>(p => p.Upvotes, user);
      await _posts.FindOneAndUpdateAsync(post => post.Id == id, removeDownvote);
      return await _posts.FindOneAndUpdateAsync(post => post.Id == id, upvote);
    }

    public async Task<Post> RemoveUpvote(string id, User user)
    {
      var removeUpvote = Builders<Post>.Update
        .PullFilter<User>(p => p.Upvotes, u => u.Id == user.Id);
      return await _posts.FindOneAndUpdateAsync(post => post.Id == id, removeUpvote);
    }

    public async Task<Post> DownvotePost(string id, User user)
    {
      var removeUpvote = Builders<Post>.Update
        .PullFilter<User>(p => p.Upvotes, u => u.Id == user.Id);
      var downvote = Builders<Post>.Update
        .AddToSet<User>(p => p.Downvotes, user);
      await _posts.FindOneAndUpdateAsync(post => post.Id == id, removeUpvote);
      return await _posts.FindOneAndUpdateAsync(post => post.Id == id, downvote);
    }

    public async Task<Post> RemoveDownvote(string id, User user)
    {
      var removeDownvote = Builders<Post>.Update
        .PullFilter<User>(p => p.Downvotes, u => u.Id == user.Id);
      return await _posts.FindOneAndUpdateAsync(post => post.Id == id, removeDownvote);
    }
 
    public int GetPostScore(string id)
    {
      return _posts.Aggregate()
        .Group(doc => doc.Id,
          group => new
          {
            Id = group.Key,
            Score = group.Sum(p => p.Upvotes.Count - p.Downvotes.Count)
          }
        ).ToList().FirstOrDefault(post => post.Id == id).Score;
    }
    public int GetPostScoreMapReduce(string id)
    {
      string mapString = 
      @"function() {
          emit(this._id, this.Upvotes.length - this.Downvotes.length)
      }";
      string reduceString = 
      @"function(id, scores) {;
        return { _id: id, score: scores }; 
      }";
      BsonJavaScript map = new BsonJavaScript(mapString);
      BsonJavaScript reduce = new BsonJavaScript(reduceString);
      var result = _posts.MapReduce<Score>(map, reduce).ToList();
      var score = Math.Floor(result.FirstOrDefault(post => post._id.ToString() == id).value);
      return Convert.ToInt32(score);
    }

    public int GetUserScoreMapReduce(string id)
    {
      string mapString =
      @"function() {
        var id = this.Author._id;
        var score = this.Upvotes.length - this.Downvotes.length;
        emit(id, score);
      }";
      string reduceString =
      @"function(id, scores) {
        return Array.sum(scores);
      }";
      BsonJavaScript map = new BsonJavaScript(mapString);
      BsonJavaScript reduce = new BsonJavaScript(reduceString);
      var result = _posts.MapReduce<Score>(map, reduce).ToList();
      var usersScore = result.FirstOrDefault(post => post._id.ToString() == id);
      var score = 0;
      if (usersScore is null)
      {
        return score;
      }
      return Convert.ToInt32(Math.Floor(usersScore.value));
    }

    private class Score
    {
      public BsonObjectId _id { get; set; }
      public double value { get; set; }
    }

    public int GetUserTotalScore(string id)
    {
      return _posts.Aggregate()
        .Group(doc => doc.Author.Id,
          group => new
          {
            Id = group.Key,
            Score = group.Sum(p => p.Upvotes.Count - p.Downvotes.Count)
          }
        ).ToList().FirstOrDefault(post => post.Id == id).Score;
    }

    public async Task RemovePost(string id) =>
        await _posts.DeleteOneAsync(post => post.Id == id);
  }
}
