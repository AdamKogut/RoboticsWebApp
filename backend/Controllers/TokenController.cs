using Backend.Messages.UserManagement;
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
      if (matchingUser != null && PasswordUtils.VerifyHash(loginMessage.Password, matchingUser.Password, loginMessage.Email))
      {
        //create claims details based on the user information
        var claims = new[] {
            new Claim("Id", Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, matchingUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, matchingUser.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: signIn);

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
      }
      else
      {
        return BadRequest("Invalid credentials");
      }
    }
  }
}