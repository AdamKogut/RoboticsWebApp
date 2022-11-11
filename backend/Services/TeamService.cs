using Backend.Enums;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Messages.UserManagement;
using Backend.Models;
using Backend.Utils;

using static Backend.Constants.ControllerFailureConstants;

namespace Backend.Services
{
  public class TeamService : ITeamService
  {
    private readonly ILogger<ITeamService> _logger;
    private readonly IUserManagementRepository _userManagementRepo;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public TeamService(
      ILogger<ITeamService> logger,
      IUserManagementRepository userManagementRepo,
      IUserService userService,
      IConfiguration configuration)
    {
      _logger = logger;
      _userManagementRepo = userManagementRepo;
      _userService = userService;
      _configuration = configuration;
    }

    public string AcceptInvite(Guid teamId, Guid userId, List<PermissionEnum> permissions)
    {
      _logger.LogTrace($"Enter TeamService.AcceptInvite");
      try
      {
        if (!_userManagementRepo.AddUserToTeam(teamId, userId))
        {
          return DatabaseFailure;
        }

        if (!_userManagementRepo.ApplyPermissions(teamId, userId, permissions))
        {
          return DatabaseFailure;
        }

        return Success;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Failure TeamService.AcceptInvite, error {e}");
        return Unknown;
      }
      finally
      {
        _logger.LogTrace($"Exit TeamService.AcceptInvite");
      }
    }

    public string ApplyPermissions(Guid teamId, Guid userId, List<UserTeamObject> permissions)
    {
      _logger.LogTrace($"Enter TeamService.ApplyPermission");
      try
      {
        if (!_userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.Admin))
        {
          return IncorrectPermissions;
        }

        foreach (var permission in permissions)
        {
          var user = _userManagementRepo.GetUser(permission.Email);

          if (!_userManagementRepo.ApplyPermissions(teamId, user.Id, permission.Permissions))
          {
            return DatabaseFailure;
          }
        }

        return Success;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Failure TeamService.ApplyPermission, error {e}");
        return Unknown;
      }
      finally
      {
        _logger.LogTrace($"Exit TeamService.ApplyPermission");
      }
    }

    //TODO: Has a lot of repo calls, may want to change in the future
    public string CreateTeam(string name, Guid userId)
    {
      _logger.LogTrace($"Enter TeamService.CreateTeam, team name {name}");
      try
      {
        if (_userManagementRepo.GetTeam(name) != null)
        {
          return TeamNameExists;
        }

        var team = new Team
        {
          Name = name
        };
        if (_userManagementRepo.GetUser(userId) == default || !_userManagementRepo.AddTeam(team))
        {
          return DatabaseFailure;
        }

        var teamId = _userManagementRepo.GetTeam(name).Id;
        if (!_userManagementRepo.AddUserToTeam(teamId, userId))
        {
          return DatabaseFailure;
        }

        var allPermissionList = new List<PermissionEnum>
        {
          PermissionEnum.Owner,
          PermissionEnum.Admin,
          PermissionEnum.AddMatchEntry,
          PermissionEnum.ReadMatchEntries,
          PermissionEnum.EditMatchEntries,
          PermissionEnum.AllowInviteToTeam
        };

        if (!_userManagementRepo.ApplyPermissions(teamId, userId, allPermissionList))
        {
          return DatabaseFailure;
        }

        return Success;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Failure TeamService.CreateTeam, error {e}");
        return Unknown;
      }
      finally
      {
        _logger.LogTrace($"Exit TeamService.CreateTeam, team name {name}");
      }
    }

    public string DeleteTeam(Guid teamId, Guid userId)
    {
      _logger.LogTrace($"Enter TeamService.DeleteTeam");
      try
      {
        if (!_userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.Owner))
        {
          return IncorrectPermissions;
        }

        if (!_userManagementRepo.DeleteTeam(teamId))
        {
          return DatabaseFailure;
        }

        return Success;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Failure TeamService.DeleteTeam, error {e}");
        return Unknown;
      }
      finally
      {
        _logger.LogTrace($"Exit TeamService.DeleteTeam");
      }
    }

    public GetInviteInfoResponse GetInviteInfo(string encryptedString)
    {
      _logger.LogTrace($"Enter TeamService.GetInviteInfo");
      var response = new GetInviteInfoResponse();
      var inviteInfo = PasswordUtils.AesDecrypt(encryptedString, _configuration).Split('\n');
      response.Email = inviteInfo[1];
      response.TeamId = new Guid(inviteInfo[0]);

      foreach (var permissionString in inviteInfo[2].Split(';'))
      {
        var permissionInt = int.Parse(permissionString);
        response.Permissions.Add((PermissionEnum)permissionInt);
      }

      return response;
    }

    public List<PermissionEnum> GetPermissions(Guid userId, Guid teamId)
    {
      _logger.LogTrace($"Enter TeamService.GetPermissions");
      return _userManagementRepo.GetPermissions(teamId, userId);
    }

    public GetTeamInfoResponse GetTeamInfo(Guid userId, Guid teamId)
    {
      _logger.LogTrace($"Enter TeamService.GetTeamInfo");
      var response = new GetTeamInfoResponse();
      var getUsersToo = _userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.Admin)
        || _userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.Owner);
      var team = _userManagementRepo.GetTeam(teamId, getUsersToo);
      if (team == default)
      {
        return response;
      }

      response.Name = team.Name;

      if (getUsersToo)
      {
        foreach (var tempUserId in team.Permissions)
        {
          var user = _userManagementRepo.GetUser(tempUserId.UserId);
          if (user == default)
          {
            continue;
          }

          response.Users.Add(new UserTeamObject
          {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Permissions = GetPermissionsFromInt(tempUserId.Permissions)
          });
        }
      }

      _logger.LogTrace($"Exit TeamService.GetTeamInfo");
      return response;
    }

    public string InviteToTeam(Guid teamId, Guid userId, string email, List<PermissionEnum> permissions)
    {
      _logger.LogTrace($"Enter TeamService.InviteToTeam");
      if (!_userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.AllowInviteToTeam))
      {
        return IncorrectPermissions;
      }

      var user = _userManagementRepo.GetUser(email);

      if (user == default)
      {
        _userService.CreateUser(string.Empty, string.Empty, email, string.Empty);
        user = _userManagementRepo.GetUser(email);
      }

      var stringToEncrypt = $"{teamId}\n{email}\n";
      foreach (var permission in permissions)
      {
        stringToEncrypt += $"{(int)permission};";
      }

      stringToEncrypt = stringToEncrypt.Substring(0, stringToEncrypt.Length - 1);
      var encrypted = PasswordUtils.AesEncrypt(stringToEncrypt, _configuration);

      if (!_userManagementRepo.GetPermissions(teamId, user.Id).Any())
      {
        // TODO: send email
      }

      _logger.LogTrace($"Exit TeamService.InviteToTeam");
      return encrypted;
    }

    public string ModifyTeam(Guid teamId, Guid userId, string teamName)
    {
      _logger.LogTrace($"Enter TeamService.ModifyTeam");
      try
      {
        if (!_userManagementRepo.CheckPermission(teamId, userId, PermissionEnum.Owner))
        {
          return IncorrectPermissions;
        }

        var oldTeam = _userManagementRepo.GetTeam(teamId, false);

        if (!string.Equals(oldTeam.Name, teamName, StringComparison.InvariantCultureIgnoreCase)
          && _userManagementRepo.GetTeam(teamName) != null)
        {
          return TeamNameExists;
        }

        oldTeam.Name = teamName;

        if (!_userManagementRepo.ModifyTeam(teamId, oldTeam))
        {
          return DatabaseFailure;
        }

        return Success;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Failure TeamService.ModifyTeam, error {e}");
        return Unknown;
      }
      finally
      {
        _logger.LogTrace($"Exit TeamService.ModifyTeam");
      }
    }

    private List<PermissionEnum> GetPermissionsFromInt(int permissionInt)
    {
      var returnList = new List<PermissionEnum>();
      foreach (PermissionEnum curr in Enum.GetValues(typeof(PermissionEnum)))
      {
        if ((permissionInt & (1 << ((int)curr - 1))) > 0)
        {
          returnList.Add(curr);
        }
      }
      return returnList;
    }
  }
}