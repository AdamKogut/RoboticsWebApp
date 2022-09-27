using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Models;
using Backend.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

    public string Authenticate(string email, string password)
    {
      _logger.LogTrace($"Enter UserService.Authenticate");
      try
      {
        var matchingUser = _userManagementRepository.GetUser(email);
        if (matchingUser != null && PasswordUtils.VerifyHash(password, matchingUser.Password, email))
        {
          matchingUser.NumberFailures = 0;
          var claims = new[] {
                      new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                      new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Ticks.ToString()),
                      new Claim("Name", email)
                  };

          var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
          var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
          var token = new JwtSecurityToken(
              _configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.UtcNow.AddMinutes(60),
              signingCredentials: signIn);

          _userManagementRepository.ModifyUser(matchingUser.Id, matchingUser);

          return new JwtSecurityTokenHandler().WriteToken(token);
        }

        return string.Empty;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Unable to authenticate, error: {e}");
        return string.Empty;
      }
      finally
      {
        _logger.LogTrace($"Exit UserService.Authenticate");
      }
    }

    public bool CreateUser(string firstName, string lastName, string email, string password, out string reason)
    {
      _logger.LogTrace($"Enter UserService.CreateUser");
      reason = string.Empty;
      try
      {
        if (_userManagementRepository.GetUser(email) != null)
        {
          reason = "UserAlreadyExists";
          return false;
        }

        var user = new User
        {
          FirstName = firstName,
          LastName = lastName,
          Email = email,
          Password = PasswordUtils.GenerateHash(password, email),
          NumberFailures = 0,
          LastFailure = DateTime.MinValue
        };
        if (!_userManagementRepository.AddUser(user))
        {
          reason = "";
          return false;
        }

        return true;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Unable to create user, error: {e}");
        reason = "Unknown";
        return false;
      }
      finally
      {
        _logger.LogTrace($"Exit UserService.CreateUser");
      }
    }

    public bool ChangePassword(Guid userId, string password, out string reason)
    {
      _logger.LogTrace($"Enter UserService.ChangePassword");
      reason = string.Empty;
      try
      {
        var user = _userManagementRepository.GetUser(userId);
        if (!_userManagementRepository.ModifyUser(userId, user))
        {
          reason = "";
          return false;
        }

        return true;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Unable to change password, error: {e}");
        reason = "Unknown";
        return false;
      }
      finally
      {
        _logger.LogTrace($"Exit UserService.ChangePassword");
      }
    }

    public bool ModifyUser(Guid userId, string firstName, string lastName, string email, out string reason)
    {
      _logger.LogTrace($"Enter UserService.ModifyUser");
      reason = string.Empty;
      try
      {
        var user = _userManagementRepository.GetUser(userId);
        if (user == null)
        {
          reason = "UserDoesntExist";
          return false;
        }

        if (user.Email.ToLower() != email.ToLower() && _userManagementRepository.GetUser(user.Email) != null)
        {
          reason = "EmailAlreadyExists";
          return false;
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;

        if (!_userManagementRepository.ModifyUser(userId, user))
        {
          reason = "DatabaseError";
          return false;
        }

        return true;
      }
      catch (System.Exception e)
      {
        _logger.LogError($"Unable to modify user, error: {e}");
        reason = "Unknown";
        return false;
      }
      finally
      {
        _logger.LogTrace($"Exit UserService.ModifyUser");
      }
    }
  }
}