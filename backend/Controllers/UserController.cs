using Backend.Interfaces.Services;
using Backend.Messages.UserManagement;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    ///   POST /api/v1/User/CreateUser
    /// </summary>
    /// <returns></returns>
    [HttpPost("CreateUser")]
    public IActionResult CreateUser(UserInfoMessage userInfoMessage)
    {
      var isSuccess = _userService.CreateUser(userInfoMessage.FirstName, userInfoMessage.LastName, userInfoMessage.Email,
        userInfoMessage.Password, out var reason);
      return Ok(reason);
    }

    /// <summary>
    ///   POST /api/v1/User/CreateTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("CreateTeam")]
    public IActionResult CreateTeam(NewTeamMessage teamInfoMessage)
    {
      var isSuccess = _teamService.CreateTeam(teamInfoMessage.Name, teamInfoMessage.UserId, out var reason);
      return Ok(reason);
    }

    /// <summary>
    ///   POST /api/v1/User/Login
    ///   TODO: on frontend save JWT in localstorage, to logout remove it from that
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserInfoMessage userInfoMessage)
    {
      var result = _userService.Authenticate(userInfoMessage.Email, userInfoMessage.Password);

      if (string.IsNullOrEmpty(result))
      {
        return Unauthorized();
      }

      return Ok(result);
    }

    /// <summary>
    ///   POST /api/v1/User/RequestChangePassword
    /// </summary>
    /// <returns></returns>
    [HttpPost("RequestChangePassword")]
    public IActionResult RequestChangePassword()
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/ChangePassword
    /// </summary>
    /// <returns></returns>
    [HttpPost("ChangePassword")]
    public IActionResult ChangePassword(UserInfoMessage userInfo)
    {
      var isSuccess = _userService.ChangePassword(userInfo.UserId, userInfo.Password, out var reason);
      return Ok(reason);
    }

    /// <summary>
    ///   POST /api/v1/User/UpdateUser
    /// </summary>
    /// <returns></returns>
    [HttpPost("UpdateUser")]
    public IActionResult UpdateUser(UserInfoMessage userInfoMessage)
    {
      var isSuccess = _userService.ModifyUser(userInfoMessage.UserId, userInfoMessage.FirstName,
        userInfoMessage.LastName, userInfoMessage.Email, out var reason);
      return Ok(reason);
    }

    /// <summary>
    ///   POST /api/v1/User/UpdateTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("UpdateTeam")]
    public IActionResult UpdateTeam(TeamInfoMessage teamInfoMessage)
    {
      var team = new Team
      {
        Id = teamInfoMessage.TeamId,
        Name = teamInfoMessage.Name
      };
      var isSuccess = _teamService.ModifyTeam(teamInfoMessage.TeamId, teamInfoMessage.UserId, team, out var reason);
      return Ok(reason);
    }

    /// <summary>
    ///   POST /api/v1/User/InviteUserToTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("InviteUserToTeam")]
    public IActionResult InviteUserToTeam(InviteUserMessage inviteMessage)
    {
      var isSuccess = _teamService.InviteToTeam(inviteMessage.TeamId, inviteMessage.UserId, inviteMessage.Email, out var reason);
      return Ok(reason);
    }

    /// <summary>
    ///   POST /api/v1/User/DeleteUser
    /// </summary>
    /// <returns></returns>
    [HttpPost("DeleteUser")]
    public IActionResult DeleteUser(UserInfoMessage userMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/DeleteTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("DeleteTeam")]
    public IActionResult DeleteTeam(TeamInfoMessage teamMessage)
    {
      var isSuccess = _teamService.DeleteTeam(teamMessage.TeamId, teamMessage.UserId, out var reason);
      return Ok(reason);
    }

    /// <summary>
    ///   POST /api/v1/User/AcceptTeamInvite
    /// </summary>
    /// <returns></returns>
    [HttpPost("AcceptTeamInvite")]
    public IActionResult AcceptTeamInvite(PermissionsInfoMessage permissionsInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/RemoveFromTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("RemoveFromTeam")]
    public IActionResult RemoveFromTeam(PermissionsInfoMessage permissionsInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/GetUsersOnTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("GetUsersOnTeam")]
    public IActionResult GetUsersOnTeam(TeamInfoMessage teamMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/ApplyPermissions
    /// </summary>
    /// <param name="permissionsInfoMessage"></param>
    /// <returns></returns>
    [HttpPost("ApplyPermissions")]
    public IActionResult ApplyPermissions(PermissionsInfoMessage permissionsInfoMessage)
    {
      return Ok();
    }
  }
}