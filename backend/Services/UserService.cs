using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Models;
using Backend.Utils;
using Backend.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using static Backend.Constants.ControllerFailureConstants;

namespace Backend.Services
{
  public class UserService : IUserService
  {
    private readonly ILogger<IUserService> _logger;
    private readonly IUserManagementRepository _userManagementRepository;
    private readonly IConfiguration _configuration;

    public UserService(ILogger<IUserService> logger, IUserManagementRepository userManagementRepository, IConfiguration configuration)
    {
      _logger = logger;
      _userManagementRepository = userManagementRepository;
      _configuration = configuration;
    }

    public string ChangePassword(string email)
    {
      _logger.LogTrace($"Enter UserService.ChangePassword");
      try
      {
        var user = _userManagementRepository.GetUser(email);
        if (user == default)
        {
          return Success;
        }

        // TODO: Send email

        return Success;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Unable to change password, error: {e}");
        return Unknown;
      }
      finally
      {
        _logger.LogTrace($"Exit UserService.ChangePassword");
      }
    }

    public string CreateUser(string firstName, string lastName, string email, string password)
    {
      _logger.LogTrace($"Enter UserService.CreateUser");
      try
      {
        if (_userManagementRepository.GetUser(email) != null)
        {
          return UserAlreadyExists;
        }

        var user = new User
        {
          FirstName = firstName,
          LastName = lastName,
          Email = email,
          Password = string.Empty,
          NumberFailures = 0,
          LastFailure = DateTime.MinValue
        };
        if (!_userManagementRepository.AddUser(user))
        {
          return DatabaseFailure;
        }

        user = _userManagementRepository.GetUser(email);
        user.Password = PasswordUtils.GenerateHash(password, user.Id);

        _userManagementRepository.ModifyUser(user.Id, user);

        return Success;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Unable to create user, error: {e}");
        return Success;
      }
      finally
      {
        _logger.LogTrace($"Exit UserService.CreateUser");
      }
    }

    public string DeleteUser(Guid userId)
    {
      _logger.LogTrace($"Enter UserService.DeleteUser");
      try
      {
        var user = _userManagementRepository.GetUser(userId);
        if (user == null)
        {
          return Unknown;
        }

        var permissions = _userManagementRepository.GetTeamsWithUser(userId);
        foreach (var permission in permissions)
        {
          if (((int)PermissionEnum.Owner & permission.Permissions) > 0)
          {
            _userManagementRepository.DeleteTeam(permission.TeamId);
          }
        }

        if (!_userManagementRepository.DeleteUser(userId))
        {
          return DatabaseFailure;
        }

        return Success;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Unable to delete user, error: {e}");
        return Unknown;
      }
      finally
      {
        _logger.LogTrace($"Exit UserService.DeleteUser");
      }
    }

    public User GetUserInfo(Guid userInfo)
    {
      _logger.LogTrace($"Enter UserService.GetUserInfo");
      try
      {
        return _userManagementRepository.GetUser(userInfo);
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Unable to get user, error: {e}");
        return new User();
      }
      finally
      {
        _logger.LogTrace($"Exit UserService.GetUserInfo");
      }
    }

    public string ModifyUser(Guid userId, string firstName, string lastName, string email, string password = "")
    {
      _logger.LogTrace($"Enter UserService.ModifyUser");
      try
      {
        var user = _userManagementRepository.GetUser(userId);
        if (user == null)
        {
          return Unknown;
        }

        if (user.Email.ToLower() != email.ToLower() && _userManagementRepository.GetUser(email) != null)
        {
          return EmailAlreadyExists;
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        if (password != string.Empty)
        {
          user.Password = PasswordUtils.GenerateHash(password, user.Id);
        }

        if (!_userManagementRepository.ModifyUser(userId, user))
        {
          return DatabaseFailure;
        }

        return Success;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Unable to modify user, error: {e}");
        return Unknown;
      }
      finally
      {
        _logger.LogTrace($"Exit UserService.ModifyUser");
      }
    }
  }
}