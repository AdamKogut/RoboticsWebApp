using Backend.Enums;
using Backend.Messages.UserManagement;
using Backend.Models;

namespace Backend.Interfaces.Services
{
  public interface ITeamService
  {
    string CreateTeam(string name, Guid userId);
    string DeleteTeam(Guid teamId, Guid userId);
    string InviteToTeam(Guid teamId, Guid userId, string email, List<PermissionEnum> permissions);
    string ModifyTeam(Guid teamId, Guid userId, string name);
    List<User> GetUsersOnTeam(Guid teamId, Guid userId);
    string ApplyPermissions(Guid teamId, Guid userId, List<UserTeamObject> permissions);
    string AcceptInvite(Guid teamId, Guid userId, List<PermissionEnum> permissions);
    List<PermissionEnum> GetPermissions(Guid userId, Guid teamId);
    GetInviteInfoResponse GetInviteInfo(string encryptedString);
    GetTeamInfoResponse GetTeamInfo(Guid userId, Guid teamId);
  }
}