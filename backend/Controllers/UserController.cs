using Backend.Enums;
using Backend.Interfaces.Services;
using Backend.Messages.UserManagement;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace Backend.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  [Authorize]
  public class UserController : Controller
  {
    private readonly ITeamService _teamService;
    private readonly IUserService _userService;

    public UserController(ITeamService teamService, IUserService userService)
    {
      _teamService = teamService;
      _userService = userService;
    }

    /// <summary>
    ///   POST /api/v1/User/AcceptTeamInvite
    /// </summary>
    /// <param name="acceptInviteMessage"></param>
    /// <returns>result string</returns>
    [HttpPost("AcceptTeamInvite")]
    public IActionResult AcceptTeamInvite(AcceptInviteMessage acceptInviteMessage)
    {
      var userId = GetUserId();
      return Ok(_teamService.AcceptInvite(acceptInviteMessage.TeamId, userId, acceptInviteMessage.Permissions));
    }

    /// <summary>
    ///   POST /api/v1/User/ChangePassword
    /// </summary>
    /// <param name="changePasswordMessage"></param>
    /// <returns>result string</returns>
    [HttpPost("ChangePassword")]
    public IActionResult ChangePassword(ChangePasswordMessage changePasswordMessage)
    {
      return Ok(_userService.ChangePassword(changePasswordMessage.Email));
    }

    /// <summary>
    ///   GET /api/v1/User/CheckPermissions
    /// </summary>
    /// <param name="teamIdMessage"></param>
    /// <returns>List of permissions</returns>
    [HttpGet("CheckPermissions")]
    public List<PermissionEnum> CheckPermissions(TeamIdMessage teamIdMessage)
    {
      var userId = GetUserId();
      return _teamService.GetPermissions(userId, teamIdMessage.TeamId);
    }

    /// <summary>
    ///   POST /api/v1/User/CreateTeam
    /// </summary>
    /// <param name="createTeamMessage"></param>
    /// <returns>result string</returns>
    [HttpPost("CreateTeam")]
    public IActionResult CreateTeam(CreateTeamMessage createTeamMessage)
    {
      var userId = GetUserId();
      return Ok(_teamService.CreateTeam(createTeamMessage.Name, userId));
    }

    /// <summary>
    ///   POST /api/v1/User/CreateUser
    /// </summary>
    /// <param name="createUserMessage"></param>
    /// <returns>result string</returns>
    [AllowAnonymous]
    [HttpPost("CreateUser")]
    public IActionResult CreateUser(CreateUserMessage createUserMessage)
    {
      return Ok(_userService.CreateUser(createUserMessage.FirstName, createUserMessage.LastName,
        createUserMessage.Email, createUserMessage.Password));
    }

    /// <summary>
    ///   POST /api/v1/User/DeleteTeam
    /// </summary>
    /// <param name="teamIdMessage"></param>
    /// <returns>result string</returns>
    [HttpPost("DeleteTeam")]
    public IActionResult DeleteTeam(TeamIdMessage teamIdMessage)
    {
      var userId = GetUserId();
      return Ok(_teamService.DeleteTeam(teamIdMessage.TeamId, userId));
    }

    /// <summary>
    ///   POST /api/v1/User/DeleteUser
    /// </summary>
    /// <returns>result string</returns>
    [HttpPost("DeleteUser")]
    public IActionResult DeleteUser()
    {
      var userId = GetUserId();
      return Ok(_userService.DeleteUser(userId));
    }

    /// <summary>
    ///   POST /api/v1/User/GetInviteInfo
    /// </summary>
    /// <param name="getInviteInfoMessage"></param>
    /// <returns>teamId, list of permissions, email</returns>
    [HttpPost("GetInviteInfo")]
    public GetInviteInfoResponse GetInviteInfo(GetInviteInfoMessage getInviteInfoMessage)
    {
      return _teamService.GetInviteInfo(getInviteInfoMessage.EncryptedString);
    }

    /// <summary>
    ///   POST /api/v1/User/GetTeamInfo
    /// </summary>
    /// <param name="teamIdMessage"></param>
    /// <returns>team name, list of users (if authorized)</returns>
    [HttpPost("GetTeamInfo")]
    public GetTeamInfoResponse GetTeamInfo(TeamIdMessage teamIdMessage)
    {
      var userId = GetUserId();
      return _teamService.GetTeamInfo(userId, teamIdMessage.TeamId);
    }

    /// <summary>
    ///   GET /api/v1/User/GetUserInfo
    /// </summary>
    /// <returns>firstname, lastname, email</returns>
    [HttpGet("GetUserInfo")]
    public GetUserResponse GetUserInfo()
    {
      var userId = GetUserId();
      var user = _userService.GetUserInfo(userId);
      return new GetUserResponse
      {
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email
      };
    }

    /// <summary>
    ///   POST /api/v1/User/InviteUserToTeam
    /// </summary>
    /// <param name="inviteMessage"></param>
    /// <returns>result string</returns>
    [HttpPost("InviteUserToTeam")]
    public IActionResult InviteUserToTeam(InviteUserMessage inviteMessage)
    {
      var userId = GetUserId();
      return Ok(_teamService.InviteToTeam(inviteMessage.TeamId, userId,
        inviteMessage.Email, inviteMessage.Permissions));
    }

    /// <summary>
    ///   POST /api/v1/User/Login
    ///   TODO: on frontend save JWT in localstorage, to logout remove it from that
    /// </summary>
    /// <param name="loginMessage"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(LoginMessage loginMessage)
    {
      var result = _userService.Authenticate(loginMessage.Email, loginMessage.Password);

      if (string.IsNullOrEmpty(result))
      {
        return Unauthorized();
      }

      return Ok(result);
    }

    /// <summary>
    ///   POST /api/v1/User/UpdateTeam
    /// </summary>
    /// <param name="updateTeamMessage"></param>
    /// <returns>result string</returns>
    [HttpPost("UpdateTeam")]
    public IActionResult UpdateTeam(UpdateTeamMessage updateTeamMessage)
    {
      var userId = GetUserId();
      return Ok(_teamService.ModifyTeam(updateTeamMessage.TeamId, userId, updateTeamMessage.Name));
    }

    /// <summary>
    ///   POST /api/v1/User/UpdateUser
    /// </summary>
    /// <param name="userInfoMessage"></param>
    /// <returns>result string</returns>
    [HttpPost("UpdateUser")]
    public IActionResult UpdateUser(UserInfoMessage userInfoMessage)
    {
      var userId = GetUserId();
      return Ok(_userService.ModifyUser(userId, userInfoMessage.FirstName,
        userInfoMessage.LastName, userInfoMessage.Email, userInfoMessage.Password));
    }

    /// <summary>
    ///   POST /api/v1/User/UpdateUserPermissions
    /// </summary>
    /// <param name="permissionsInfoMessage"></param>
    /// <returns>result string</returns>
    [HttpPost("UpdateUserPermissions")]
    public IActionResult UpdateUserPermissions(PermissionsInfoMessage permissionsInfoMessage)
    {
      var userId = GetUserId();
      return Ok(_teamService.ApplyPermissions(permissionsInfoMessage.TeamId, userId, permissionsInfoMessage.Users));
    }

    private Guid GetUserId()
    {
      var token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      var handler = new JwtSecurityTokenHandler();
      var jwtSecurityToken = handler.ReadJwtToken(token);
      var userId = jwtSecurityToken.Claims.First(claim => claim.Type == "UserId").Value;
      return new Guid(userId);
    }
  }
}