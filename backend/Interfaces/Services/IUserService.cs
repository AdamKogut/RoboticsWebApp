using Backend.Enums;
using Backend.Models;

namespace Backend.Interfaces.Services
{
  public interface IUserService
  {
    bool CreateUser(string firstName, string lastName, Guid userId, out string reason);
    string Authenticate(string email, string password);
    bool DeleteUser(Guid userId, out string reason);
    bool InviteToTeam(Guid teamId, string email, out string reason);
    bool ModifyTeam(Guid teamId, Guid userId, Team newTeam, out string reason);
  }
}