using MongoDB.Driver;
using MongoDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.Services
{
  public class UsersService
  {
    private readonly IMongoCollection<User> _users;

    public UsersService(IMongoCollection<User> users)
    {
      _users = users;
    }

    public async Task<List<User>> GetAllUsers()
    {
      var users = await _users.FindAsync(user => true);
      return await users.ToListAsync();
    }

    public async Task<User> GetUserById(string id)
    {
      var users = await _users.FindAsync<User>(user => user.Id == id);
      return await users.FirstOrDefaultAsync();
    }

    public async Task<User> CreateUser(User user)
    {
      await _users.InsertOneAsync(user);
      return user;
    }

    public async Task RemoveUser(string id) =>
        await _users.DeleteOneAsync(report => report.Id == id);
  }
}
