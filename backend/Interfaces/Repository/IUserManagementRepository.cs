using Backend.Enums;
using Backend.Models;

namespace Backend.Interfaces.Repository
{
  public interface IUserManagementRepository
  {
    bool AddUser(User user);
    bool DeleteUser(Guid userId);
    User GetUser(Guid userId);
    User GetUser(string email);
    bool ModifyUser(Guid userId, User newInfo);
    bool AddTeam(Team team);
    bool DeleteTeam(Guid teamId);
    Team GetTeam(Guid teamId);
    Team GetTeam(string name);
    bool ModifyTeam(Guid teamId, Team newInfo);
    bool ApplyPermissions(Guid teamId, Guid userId, List<PermissionEnum> permissionEnum);
    List<PermissionEnum> GetPermissions(Guid teamId, Guid userId);
    bool AddUserToTeam(Guid teamId, Guid userId);
    bool CheckPermission(Guid teamId, Guid userId, PermissionEnum permissionEnum);
    List<User> GetUsersOnTeam(Guid teamId);
  }
}