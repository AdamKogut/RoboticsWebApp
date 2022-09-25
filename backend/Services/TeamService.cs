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

        if (!_userManagementRepo.AddPermission(teamId, userId, PermissionEnum.Owner))
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

    public bool InviteToTeam(Guid teamId, string email, out string reason)
    {
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
  }
}