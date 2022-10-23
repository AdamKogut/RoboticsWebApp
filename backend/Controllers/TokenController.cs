using Backend.Messages.UserManagement;
using Backend.Models;
using Backend.Utils;
using Backend.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuth.WebApi.Controllers
{
  [Route("api/token")]
  [ApiController]
  public class TokenController : ControllerBase
  {
    private readonly ILogger<TokenController> _logger;
    private readonly IUserManagementRepository _userManagementRepository;
    private readonly IConfiguration _configuration;

    public TokenController(ILogger<TokenController> logger, IUserManagementRepository userManagementRepository, IConfiguration configuration)
    {
      _logger = logger;
      _userManagementRepository = userManagementRepository;
      _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Post(LoginMessage loginMessage)
    {
      var matchingUser = _userManagementRepository.GetUser(loginMessage.Email);
      if (matchingUser != null && !CheckIfAbleToLogin(matchingUser))
      {
        return Ok("NeedToWait");
      }
      if (matchingUser != null && PasswordUtils.VerifyHash(loginMessage.Password, matchingUser.Password, matchingUser.Id))
      {
        //create claims details based on the user information
        var claims = new[] {
          new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
          new Claim(JwtRegisteredClaimNames.Email, matchingUser.Email),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Ticks.ToString()),
          new Claim("UserId", matchingUser.Id.ToString())
      };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: signIn);

        matchingUser.LastFailure = DateTime.MinValue;
        matchingUser.NumberFailures = 0;
        _userManagementRepository.ModifyUser(matchingUser.Id, matchingUser, true);

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
      }
      else
      {
        if (matchingUser != null)
        {
          matchingUser.NumberFailures++;
          matchingUser.LastFailure = DateTime.UtcNow;
          _userManagementRepository.ModifyUser(matchingUser.Id, matchingUser, true);
        }

        return Unauthorized("Invalid credentials");
      }
    }

    private bool CheckIfAbleToLogin(User user)
    {
      if (user.NumberFailures > 3)
      {
        if (DateTime.UtcNow < user.LastFailure.AddMinutes(Math.Pow(2, user.NumberFailures - 3)))
        {
          return false;
        }
      }

      return true;
    }
  }
}