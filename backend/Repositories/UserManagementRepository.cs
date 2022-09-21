using Backend.Enums;
using Backend.Interfaces;
using Backend.Models;

namespace Backend.Repositories
{
  public class UserManagementRepository : IUserManagementRepository
  {
    private IServiceScopeFactory _scopeFactory;
    private ILogger<UserManagementRepository> _logger;
    private object _userLock;
    private object _teamLock;
    private object _permissionLock;

    public UserManagementRepository(IServiceScopeFactory scopeFactory, ILogger<UserManagementRepository> logger)
    {
      _scopeFactory = scopeFactory;
      _logger = logger;
    }

    public bool AddUser(User user)
    {
      _logger.LogTrace("Enter UserManagementRepository.AddUser");
      try
      {
        lock (_userLock)
        {
          using (var scope = _scopeFactory.CreateScope())
          {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Users.Add(user);
            db.SaveChanges();
          }
        }
        return true;
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when adding new user. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.AddUser");
      }
    }

    public User GetUser(Guid userId)
    {
      _logger.LogTrace("Enter UserManagementRepository.GetUser");
      try
      {
        lock (_userLock)
        {
          using (var scope = _scopeFactory.CreateScope())
          {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return db.Users.FirstOrDefault(x => x.Id == userId)!;
          }
        }
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when getting user. Error: {e}");
        return null!;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.GetUser");
      }
    }

    public bool DeleteUser(Guid userId)
    {
      _logger.LogTrace("Enter UserManagementRepository.DeleteUser");
      try
      {
        lock (_userLock)
        {
          using (var scope = _scopeFactory.CreateScope())
          {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
              return false;
            }
            db.Users.Remove(user);
            db.SaveChanges();
          }
        }
        return true;
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when removing user. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.DeleteUser");
      }
    }

    public bool AddTeam(Team team)
    {
      _logger.LogTrace("Enter UserManagementRepository.AddTeam");
      try
      {
        lock (_teamLock)
        {
          using (var scope = _scopeFactory.CreateScope())
          {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Teams.Add(team);
            db.SaveChanges();
          }
        }
        return true;
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when adding new team. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.AddTeam");
      }
    }

    public Team GetTeam(Guid teamId)
    {
      _logger.LogTrace("Enter UserManagementRepository.GetTeam");
      try
      {
        lock (_teamLock)
        {
          using (var scope = _scopeFactory.CreateScope())
          {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return db.Teams.FirstOrDefault(x => x.Id == teamId)!;
          }
        }
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when getting team. Error: {e}");
        return null!;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.GetTeam");
      }
    }

    public bool DeleteTeam(Guid teamId)
    {
      _logger.LogTrace("Enter UserManagementRepository.DeleteTeam");
      try
      {
        lock (_teamLock)
        {
          using (var scope = _scopeFactory.CreateScope())
          {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var team = db.Teams.FirstOrDefault(x => x.Id == teamId);
            if (team == null)
            {
              return false;
            }
            db.Teams.Remove(team);
            db.SaveChanges();
          }
        }
        return true;
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when removing team. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.DeleteTeam");
      }
    }

    public bool AddPermission(Guid teamId, Guid userId, PermissionEnum permissionEnum)
    {
      _logger.LogTrace("Enter UserManagementRepository.AddPermission");
      try
      {
        lock (_permissionLock) lock (_userLock) lock (_teamLock)
            {
              using (var scope = _scopeFactory.CreateScope())
              {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var permission = db.Permissions.FirstOrDefault(x => x.TeamId == teamId && x.UserId == userId);
                if (permission == default)
                {
                  return false;
                }
                permission.Permissions = 1 << (int)permissionEnum | permission.Permissions;
                db.SaveChanges();
              }
            }
        return true;
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when adding permission to user. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.AddPermission");
      }
    }

    public bool RemovePermission(Guid teamId, Guid userId, PermissionEnum permissionEnum)
    {
      _logger.LogTrace("Enter UserManagementRepository.RemovePermission");
      try
      {
        lock (_permissionLock) lock (_userLock) lock (_teamLock)
            {
              using (var scope = _scopeFactory.CreateScope())
              {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var permission = db.Permissions.FirstOrDefault(x => x.TeamId == teamId && x.UserId == userId);
                if (permission == default)
                {
                  return false;
                }
                permission.Permissions = ~(1 << (int)permissionEnum) & permission.Permissions;
                db.SaveChanges();
              }
            }
        return true;
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when removing permission from user. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.RemovePermission");
      }
    }

    public List<PermissionEnum> GetPermissions(Guid teamId, Guid userId)
    {
      _logger.LogTrace("Enter UserManagementRepository.GetPermissions");
      try
      {
        lock (_permissionLock) lock (_userLock) lock (_teamLock)
            {
              using (var scope = _scopeFactory.CreateScope())
              {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var permission = db.Permissions.FirstOrDefault(x => x.TeamId == teamId && x.UserId == userId);
                if (permission == default)
                {
                  return new List<PermissionEnum>();
                }

                var returnList = new List<PermissionEnum>();
                foreach (PermissionEnum curr in Enum.GetValues(typeof(PermissionEnum)))
                {
                  if ((permission.Permissions & (1 << ((int)curr - 1))) == 1)
                  {
                    returnList.Add(curr);
                  }
                }

                return returnList;
              }
            }
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when getting permissions for user. Error: {e}");
        return new List<PermissionEnum>();
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.GetPermissions");
      }
    }
  }
}