using Backend.Enums;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Models;

namespace Backend.Services
{
  public class TeamService : ITeamService
  {
    private readonly ILogger<ITeamService> _logger;
    private readonly IUserManagementRepository _userManagementRepo;

    public TeamService(ILogger<ITeamService> logger, IUserManagementRepository userManagementRepo)
    {
      _logger = logger;
      _userManagementRepo = userManagementRepo;
    }

    //TODO: Has a lot of repo calls, may want to change in the future
    public bool CreateTeam(string name, Guid userId, out string reason)
    {
      _logger.LogTrace($"Enter TeamService.CreateTeam, team name {name}");
      reason = "";
      try
      {
        if (_userManagementRepo.GetTeam(name) != null)
        {
          reason = "TeamNameExists";
          return false;
        }

        var team = new Team
        {
          Name = name
        };
        if (!_userManagementRepo.AddTeam(team))
        {
          reason = "DatabaseFailure";
          return false;
        }

        var teamId = _userManagementRepo.GetTeam(name).Id;
        if (!_userManagementRepo.AddUserToTeam(teamId, userId))
        {
          reason = "DatabaseFailure";
          return false;
        }

        if (!_userManagementRepo.AddPermission(teamId, userId, PermissionEnum.Owner)
          || !_userManagementRepo.AddPermission(teamId, userId, PermissionEnum.Admin))
        {
          reason = "DatabaseFailure";
          return false;
        }

        return true;
      }
      catch (System.Exception e)
      {
        reason = "Unknown";
        _logger.LogError($"Failure TeamService.CreateTeam, error {e}");
        return false;
      }
      finally
      {
        var result = string.IsNullOrEmpty(reason) ? "success" : $"failure, reason {reason}";
        _logger.LogTrace($"Exit TeamService.CreateTeam, team name {name}, result {result}");
      }
    }

    public bool DeleteTeam(Guid teamId, Guid userId, out string reason)
    {
      _logger.LogTrace($"Enter TeamService.DeleteTeam");
      reason = "";
      try
      {
        if (!_userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.Owner))
        {
          reason = "NotOwner";
          return false;
        }

        if (!_userManagementRepo.DeleteTeam(teamId))
        {
          reason = "DatabaseFailure";
          return false;
        }

        return true;
      }
      catch (System.Exception e)
      {
        reason = "Unknown";
        _logger.LogError($"Failure TeamService.DeleteTeam, error {e}");
        return false;
      }
      finally
      {
        var result = string.IsNullOrEmpty(reason) ? "success" : $"failure, reason {reason}";
        _logger.LogTrace($"Exit TeamService.DeleteTeam, result {result}");
      }
    }

    public bool InviteToTeam(Guid teamId, Guid userId, string email, out string reason)
    {
      if (!_userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.AllowInviteToTeam))
      {
        reason = "IncorrectPermissions";
        return false;
      }
      // TODO: set up sending email
      throw new NotImplementedException();
    }

    public bool ModifyTeam(Guid teamId, Guid userId, Team newTeam, out string reason)
    {
      _logger.LogTrace($"Enter TeamService.ModifyTeam");
      reason = "";
      try
      {
        if (!_userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.Owner))
        {
          reason = "NotOwner";
          return false;
        }

        var oldTeam = _userManagementRepo.GetTeam(teamId);

        if (!string.Equals(oldTeam.Name, newTeam.Name, StringComparison.InvariantCultureIgnoreCase)
          && _userManagementRepo.GetTeam(newTeam.Name) != null)
        {
          reason = "TeamExists";
          return false;
        }

        if (!_userManagementRepo.ModifyTeam(teamId, newTeam))
        {
          reason = "DatabaseFailure";
          return false;
        }

        return true;
      }
      catch (System.Exception e)
      {
        reason = "Unknown";
        _logger.LogError($"Failure TeamService.ModifyTeam, error {e}");
        return false;
      }
      finally
      {
        var result = string.IsNullOrEmpty(reason) ? "success" : $"failure, reason {reason}";
        _logger.LogTrace($"Exit TeamService.ModifyTeam, result {result}");
      }
    }

    public bool RemoveFromTeam(Guid teamId, Guid requesterId, Guid removeUserId, out string reason)
    {
      _logger.LogTrace($"Enter TeamService.RemoveFromTeam");
      reason = "";
      try
      {
        if (!_userManagementRepo.CheckPermission(teamId, requesterId, PermissionEnum.Admin))
        {
          reason = "NotAdmin";
          return false;
        }

        if (!_userManagementRepo.DeletePermission(teamId, removeUserId))
        {
          reason = "DatabaseFailure";
          return false;
        }

        return true;
      }
      catch (System.Exception e)
      {
        reason = "Unknown";
        _logger.LogError($"Failure TeamService.RemoveFromTeam, error {e}");
        return false;
      }
      finally
      {
        var result = string.IsNullOrEmpty(reason) ? "success" : $"failure, reason {reason}";
        _logger.LogTrace($"Exit TeamService.RemoveFromTeam, result {result}");
      }
    }

    public List<User> GetUsersOnTeam(Guid teamId, Guid userId)
    {
      _logger.LogTrace($"Enter TeamService.GetUsers");
      try
      {
        if (!_userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.Admin))
        {
          return new List<User>();
        }

        return _userManagementRepo.GetUsersOnTeam(teamId);
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Failure TeamService.GetUsers, error {e}");
        return new List<User>();
      }
      finally
      {
        _logger.LogTrace($"Exit TeamService.GetUsers");
      }
    }

    public bool ApplyPermission(Guid teamId, Guid requesterId, Guid userId, List<PermissionEnum> permissions, out string reason)
    {
      _logger.LogTrace($"Enter TeamService.ApplyPermission");
      reason = "";
      try
      {
        if (!_userManagementRepo.CheckPermission(teamId, requesterId, PermissionEnum.Admin))
        {
          reason = "NotAdmin";
          return false;
        }

        foreach (var permission in permissions)
        {
          if (!_userManagementRepo.AddPermission(teamId, userId, permission))
          {
            reason = "DatabaseFailure";
            return false;
          }
        }

        return true;
      }
      catch (System.Exception e)
      {
        reason = "Unknown";
        _logger.LogError($"Failure TeamService.ApplyPermission, error {e}");
        return false;
      }
      finally
      {
        var result = string.IsNullOrEmpty(reason) ? "success" : $"failure, reason {reason}";
        _logger.LogTrace($"Exit TeamService.ApplyPermission, result {result}");
      }
    }

    public bool AcceptInvite(Guid teamId, Guid userId, List<PermissionEnum> permissions, out string reason)
    {
      _logger.LogTrace($"Enter TeamService.AcceptInvite");
      reason = "";
      try
      {
        if (!_userManagementRepo.AddUserToTeam(teamId, userId))
        {
          reason = "DatabaseFailure";
          return false;
        }

        foreach (var permission in permissions)
        {
          if (!_userManagementRepo.AddPermission(teamId, userId, permission))
          {
            reason = "DatabaseFailure";
            return false;
          }
        }

        return true;
      }
      catch (System.Exception e)
      {
        reason = "Unknown";
        _logger.LogError($"Failure TeamService.AcceptInvite, error {e}");
        return false;
      }
      finally
      {
        var result = string.IsNullOrEmpty(reason) ? "success" : $"failure, reason {reason}";
        _logger.LogTrace($"Exit TeamService.AcceptInvite, result {result}");
      }
    }
  }
}