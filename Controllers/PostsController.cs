using Microsoft.AspNetCore.Mvc;
using MongoDB.Models;
using MongoDB.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PostsController : ControllerBase
  {
    private readonly PostsService _postsService;
    private readonly UsersService _usersService;
    public PostsController(PostsService postsService, UsersService usersService)
    {
      _postsService = postsService;
      _usersService = usersService;
  }

    [HttpGet]
    public async Task<ActionResult<List<Post>>> GetAllPosts()
    {
      return await _postsService.GetAllPosts();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Post>> GetPost(string id)
    {
      return await _postsService.GetPostById(id);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost(Post post)
    {
      return await _postsService.CreatePost(post);
    }

    [HttpPatch("addComment/{id:length(24)}")]
    public async Task<ActionResult<Post>> AddComment(string id, Comment comment)
    {
      return await _postsService.AddComment(id, comment);
    }

    [HttpPatch("removeComment/{postId:length(24)}/{commentId:length(24)}")]
    public async Task<ActionResult<Post>> RemoveComment(string postId, string commentId)
    {
      return await _postsService.RemoveComment(postId, commentId);
    }

    [HttpPatch("upvote/{postId:length(24)}/{userId:length(24)}")]
    public async Task<ActionResult<Post>> UpvotePost(string postId, string userId)
    {
      var user = await _usersService.GetUserById(userId);
      return await _postsService.UpvotePost(postId, user);
    }

    [HttpPatch("removeUpvote/{postId:length(24)}/{userId:length(24)}")]
    public async Task<ActionResult<Post>> RemoveUpvote(string postId, string userId)
    {
      var user = await _usersService.GetUserById(userId);
      return await _postsService.RemoveUpvote(postId, user);
    }

    [HttpPatch("downvote/{postId:length(24)}/{userId:length(24)}")]
    public async Task<ActionResult<Post>> DownvotePost(string postId, string userId)
    {
      var user = await _usersService.GetUserById(userId);
      return await _postsService.DownvotePost(postId, user);
    }

    [HttpPatch("removeDownvote/{postId:length(24)}/{userId:length(24)}")]
    public async Task<ActionResult<Post>> RemoveDownvote(string postId, string userId)
    {
      var user = await _usersService.GetUserById(userId);
      return await _postsService.RemoveDownvote(postId, user);
    }
    [HttpGet("score/{id:length(24)}")]
    public ActionResult<int> GetPostScore(string id)
    {
      var post = _postsService.GetPostById(id);
      if (post is null)
      {
        return BadRequest("Įrašas su tokiu id nerastas");
      }
      return _postsService.GetPostScore(id);
    }

    [HttpGet("scoreMR/{id:length(24)}")]
    public ActionResult<int> GetPostScoreMR(string id)
    {
      var post = _postsService.GetPostById(id);
      if (post is null)
      {
        return BadRequest("Įrašas su tokiu id nerastas");
      }
      return _postsService.GetPostScoreMapReduce(id);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<ActionResult> DeletePost(string id)
    {
      var post = await _postsService.GetPostById(id);

      if (post == null)
      {
        return NotFound();
      }

      await _postsService.RemovePost(id);
      return NoContent();
    }
  }
}
