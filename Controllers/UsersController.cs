using Microsoft.AspNetCore.Mvc;
using MongoDB.Models;
using MongoDB.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly UsersService _usersService;
    private readonly PostsService _postsService;
    public UsersController(UsersService usersService, PostsService postsService)
    {
      _usersService = usersService;
      _postsService = postsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
      return await _usersService.GetAllUsers();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> GetUser(string id)
    {
      return await _usersService.GetUserById(id);
    }

    [HttpGet("score/{id:length(24)}")]
    public ActionResult<int> GetUserScore(string id)
    {
      var user = _usersService.GetUserById(id);
      if (user is null)
      {
        return BadRequest("Vartotojas su tokiu id nerastas");
      }
      return _postsService.GetUserTotalScore(id);
    }

    [HttpGet("scoreMR/{id:length(24)}")]
    public ActionResult<int> GetUserScoreMapReduce(string id)
    {
      var user = _usersService.GetUserById(id);
      if (user is null)
      {
        return BadRequest("Vartotojas su tokiu id nerastas");
      }
      return _postsService.GetUserScoreMapReduce(id);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
      return await _usersService.CreateUser(user);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
      var post = await _usersService.GetUserById(id);

      if (post == null)
      {
        return NotFound();
      }

      await _usersService.RemoveUser(id);
      return NoContent();
    }
  }
}
