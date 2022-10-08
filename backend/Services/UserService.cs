using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Models;
using Backend.Utils;
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

    public string Authenticate(string email, string password)
    {
      _logger.LogTrace($"Enter UserService.Authenticate");
      try
      {
        var matchingUser = _userManagementRepository.GetUser(email);
        if (matchingUser != null && PasswordUtils.VerifyHash(password, matchingUser.Password, email))
        {
          matchingUser.NumberFailures = 0;

          var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
          var tokenDescriptor = new SecurityTokenDescriptor
          {
            Subject = new ClaimsIdentity(new[]
              {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, matchingUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, matchingUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
             }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials
              (new SymmetricSecurityKey(key),
              SecurityAlgorithms.HmacSha512Signature)
          };
          var tokenHandler = new JwtSecurityTokenHandler();
          var token = tokenHandler.CreateToken(tokenDescriptor);
          var jwtToken = tokenHandler.WriteToken(token);
          var stringToken = tokenHandler.WriteToken(token);


          // var claims = new[] {
          //             new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
          //             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          //             new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Ticks.ToString()),
          //             new Claim("UserId", matchingUser.Id.ToString())
          //         };

          // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
          // var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
          // var token = new JwtSecurityToken(
          //     _configuration["Jwt:Issuer"],
          //     _configuration["Jwt:Audience"],
          //     claims,
          //     expires: DateTime.UtcNow.AddMinutes(60),
          //     signingCredentials: signIn);

          _userManagementRepository.ModifyUser(matchingUser.Id, matchingUser);
          return stringToken;

          // return new JwtSecurityTokenHandler().WriteToken(token);
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
          Password = PasswordUtils.GenerateHash(password, email),
          NumberFailures = 0,
          LastFailure = DateTime.MinValue
        };
        if (!_userManagementRepository.AddUser(user))
        {
          return DatabaseFailure;
        }

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
        _logger.LogError($"Unable to modify user, error: {e}");
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

        if (user.Email.ToLower() != email.ToLower() && _userManagementRepository.GetUser(user.Email) != null)
        {
          return EmailAlreadyExists;
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        if (password != string.Empty)
        {
          user.Password = PasswordUtils.GenerateHash(password, email);
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