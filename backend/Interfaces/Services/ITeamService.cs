using Backend.Models;

namespace Backend.Interfaces.Services
{
  public interface ITeamService
  {
    bool CreateTeam(string name, Guid userId, out string reason);
    bool DeleteTeam(Guid teamId, Guid userId, out string reason);
    bool InviteToTeam(Guid teamId, string email, out string reason);
    bool ModifyTeam(Guid teamId, Guid userId, Team newTeam, out string reason);
  }
}