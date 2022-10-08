using Backend.Enums;
using Backend.Interfaces.Repository;
using Backend.Models;

namespace Backend.Repositories
{
  //TODO: Force AddUser and AddTeam to error if user with same email exists
  //TODO: Create UpdateUser and UpdateTeam
  public class UserManagementRepository : IUserManagementRepository
  {
    private IServiceScopeFactory _scopeFactory;
    private ILogger<UserManagementRepository> _logger;
    private object _userLock = new object();
    private object _teamLock = new object();
    private object _permissionLock = new object();

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

    public User GetUser(string email)
    {
      _logger.LogTrace("Enter UserManagementRepository.GetUser");
      try
      {
        lock (_userLock)
        {
          using (var scope = _scopeFactory.CreateScope())
          {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return db.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower())!;
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

    public bool ModifyUser(Guid userId, User newInfo)
    {
      _logger.LogTrace("Enter UserManagementRepository.ModifyUser");
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

            user.FirstName = newInfo.FirstName;
            user.LastName = newInfo.LastName;
            user.Email = newInfo.Email;
            user.Password = newInfo.Password;

            db.SaveChanges();
          }
        }
        return true;
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when modifying user. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.ModifyUser");
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

    public Team GetTeam(string name)
    {
      _logger.LogTrace("Enter UserManagementRepository.GetTeam");
      try
      {
        lock (_teamLock)
        {
          using (var scope = _scopeFactory.CreateScope())
          {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return db.Teams.FirstOrDefault(x => x.Name.ToLower() == name.ToLower())!;
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

    public bool ModifyTeam(Guid teamId, Team newInfo)
    {
      _logger.LogTrace("Enter UserManagementRepository.ModifyTeam");
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

            team.Name = newInfo.Name;
            db.SaveChanges();
          }
        }
        return true;
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when modifing team. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.ModifyTeam");
      }
    }

    public bool ApplyPermissions(Guid teamId, Guid userId, List<PermissionEnum> permissionEnum)
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
                  db.Permissions.Add(new Permission
                  {
                    TeamId = teamId,
                    UserId = userId,
                    Permissions = ConvertPermissionsToInt(permissionEnum)
                  });
                }
                else
                {
                  permission.Permissions = ConvertPermissionsToInt(permissionEnum);
                }
                db.SaveChanges();
              }
              return true;
            }
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when getting permissions for user. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.GetPermissions");
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
                  if ((permission.Permissions & (1 << ((int)curr))) == 1)
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

    public bool AddUserToTeam(Guid teamId, Guid userId)
    {
      _logger.LogTrace("Enter UserManagementRepository.AddUserToTeam");
      try
      {
        lock (_permissionLock) lock (_userLock) lock (_teamLock)
            {
              using (var scope = _scopeFactory.CreateScope())
              {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var user = db.Users.FirstOrDefault(x => x.Id == userId);
                var team = db.Teams.FirstOrDefault(x => x.Id == teamId);
                if (user == default || team == default)
                {
                  _logger.LogError($"Unable to add user to team because user or team do not exist");
                  return false;
                }
                var permission = db.Permissions.FirstOrDefault(x => x.TeamId == teamId && x.UserId == userId);
                if (permission == default)
                {
                  permission = new Permission
                  {
                    UserId = userId,
                    TeamId = teamId,
                    Permissions = 0,
                    User = user,
                    Team = team
                  };
                  db.Permissions.Add(permission);
                }

                return true;
              }
            }
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when adding user to team. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.AddUserToTeam");
      }
    }

    public bool CheckPermission(Guid teamId, Guid userId, PermissionEnum permissionEnum)
    {
      _logger.LogTrace("Enter UserManagementRepository.CheckPermission");
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

                if ((permission.Permissions & (1 << ((int)permissionEnum - 1))) == 1)
                {
                  return true;
                }

                return false;
              }
            }
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when getting permissions for user. Error: {e}");
        return false;
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.CheckPermission");
      }
    }

    public List<User> GetUsersOnTeam(Guid teamId)
    {
      _logger.LogTrace("Enter UserManagementRepository.GetUsersOnTeam");
      try
      {
        lock (_permissionLock) lock (_userLock) lock (_teamLock)
            {
              using (var scope = _scopeFactory.CreateScope())
              {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var permissions = db.Permissions.Where(x => x.TeamId == teamId);

                return permissions.Select(x => x.User).ToList();
              }
            }
      }
      catch (Exception e)
      {
        _logger.LogError($"Error occured when deleting permissions for user. Error: {e}");
        return new List<User>();
      }
      finally
      {
        _logger.LogTrace("Exit UserManagementRepository.GetUsersOnTeam");
      }
    }

    private int ConvertPermissionsToInt(List<PermissionEnum> permissions)
    {
      var returnInt = 0;
      foreach (var permission in permissions)
      {
        returnInt |= (1 << ((int)permission));
      }
      return returnInt;
    }
  }
}