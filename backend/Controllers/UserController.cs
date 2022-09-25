using Backend.Interfaces.Services;
using Backend.Messages.UserManagement;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class UserController : Controller
  {
    private readonly ITeamService _teamService;

    public UserController(ITeamService teamService)
    {
      _teamService = teamService;
    }

    /// <summary>
    ///   POST /api/v1/User/CreateUser
    /// </summary>
    /// <returns></returns>
    [HttpPost("CreateUser")]
    public ActionResult CreateUser(UserInfoMessage userInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/CreateTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("CreateTeam")]
    public ActionResult CreateTeam(NewTeamMessage teamInfoMessage)
    {
      var isSuccess = _teamService.CreateTeam(teamInfoMessage.Name, teamInfoMessage.UserId, out var reason);
      return Ok(reason);
    }

    /// <summary>
    ///   POST /api/v1/User/Login
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("Login")]
    public ActionResult Login(UserInfoMessage userInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/ChangePassword
    /// </summary>
    /// <returns></returns>
    [HttpPost("ChangePassword")]
    public ActionResult ChangePassword()
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/Logout
    /// </summary>
    /// <returns></returns>
    [HttpPost("Logout")]
    public ActionResult Logout()
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/UpdateUser
    /// </summary>
    /// <returns></returns>
    [HttpPost("UpdateUser")]
    public ActionResult UpdateUser(UserInfoMessage userInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/UpdateTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("UpdateTeam")]
    public ActionResult UpdateTeam(TeamInfoMessage teamInfoMessage)
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
    public ActionResult InviteUserToTeam()
    {
      return Ok(); //Sends invite over email
    }

    /// <summary>
    ///   POST /api/v1/User/DeleteUser
    /// </summary>
    /// <returns></returns>
    [HttpPost("DeleteUser")]
    public ActionResult DeleteUser(UserInfoMessage userMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/DeleteTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("DeleteTeam")]
    public ActionResult DeleteTeam(TeamInfoMessage teamMessage)
    {
      var isSuccess = _teamService.DeleteTeam(teamMessage.TeamId, teamMessage.UserId, out var reason);
      return Ok(reason);
    }

    /// <summary>
    ///   POST /api/v1/User/AcceptTeamInvite
    /// </summary>
    /// <returns></returns>
    [HttpPost("AcceptTeamInvite")]
    public ActionResult AcceptTeamInvite(PermissionsInfoMessage permissionsInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/RemoveFromTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("RemoveFromTeam")]
    public ActionResult RemoveFromTeam(PermissionsInfoMessage permissionsInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/GetUsersOnTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost("GetUsersOnTeam")]
    public ActionResult GetUsersOnTeam(TeamInfoMessage teamMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/ApplyPermissions
    /// </summary>
    /// <param name="permissionsInfoMessage"></param>
    /// <returns></returns>
    [HttpPost("ApplyPermissions")]
    public ActionResult ApplyPermissions(PermissionsInfoMessage permissionsInfoMessage)
    {
      return Ok();
    }
  }
}