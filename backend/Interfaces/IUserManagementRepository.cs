using Backend.Enums;
using Backend.Models;

namespace Backend.Interfaces
{
  public interface IUserManagementRepository
  {
    bool AddUser(User user);
    bool DeleteUser(Guid userId);
    User GetUser(Guid userId);
    bool AddTeam(Team team);
    bool DeleteTeam(Guid teamId);
    Team GetTeam(Guid teamId);
    bool AddPermission(Guid teamId, Guid userId, PermissionEnum permissionEnum);
    bool RemovePermission(Guid teamId, Guid userId, PermissionEnum permissionEnum);
    List<PermissionEnum> GetPermissions(Guid teamId, Guid userId);
    bool AddUserToTeam(Guid teamId, Guid userId);
    bool CheckPermission(Guid teamId, Guid userId, PermissionEnum permissionEnum);
    bool DeletePermission(Guid teamId, Guid userId);
    List<User> GetUsersOnTeam(Guid teamId);
  }
}