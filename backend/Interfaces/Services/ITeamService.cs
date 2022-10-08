using Backend.Enums;
using Backend.Messages.UserManagement;

namespace Backend.Interfaces.Services
{
  public interface ITeamService
  {
    string AcceptInvite(Guid teamId, Guid userId, List<PermissionEnum> permissions);
    string ApplyPermissions(Guid teamId, Guid userId, List<UserTeamObject> permissions);
    string CreateTeam(string name, Guid userId);
    string DeleteTeam(Guid teamId, Guid userId);
    GetInviteInfoResponse GetInviteInfo(string encryptedString);
    List<PermissionEnum> GetPermissions(Guid userId, Guid teamId);
    GetTeamInfoResponse GetTeamInfo(Guid userId, Guid teamId);
    string InviteToTeam(Guid teamId, Guid userId, string email, List<PermissionEnum> permissions);
    string ModifyTeam(Guid teamId, Guid userId, string name);
  }
}