using Backend.Messages.UserManagement;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class UserController : Controller
  {
    public UserController()
    {

    }

    /// <summary>
    ///   POST /api/v1/User/CreateUser
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult CreateUser(UserInfoMessage userInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/CreateTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult CreateTeam(TeamInfoMessage teamInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/Login
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult Login(UserInfoMessage userInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/ChangePassword
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult ChangePassword()
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/Logout
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult Logout()
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/UpdateUser
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult UpdateUser(UserInfoMessage userInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/UpdateTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult UpdateTeam(TeamInfoMessage teamInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/InviteUserToTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult InviteUserToTeam()
    {
      return Ok(); //Sends invite over email
    }

    /// <summary>
    ///   POST /api/v1/User/DeleteUser
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult DeleteUser(UserInfoMessage userMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/DeleteTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult DeleteTeam(TeamInfoMessage teamMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/AcceptTeamInvite
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult AcceptTeamInvite(PermissionsInfoMessage permissionsInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/RemoveFromTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult RemoveFromTeam(PermissionsInfoMessage permissionsInfoMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/GetUsersOnTeam
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult GetUsersOnTeam(TeamInfoMessage teamMessage)
    {
      return Ok();
    }

    /// <summary>
    ///   POST /api/v1/User/GetUsersOnTeam
    /// </summary>
    /// <param name="permissionsInfoMessage"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult ApplyPermissions(PermissionsInfoMessage permissionsInfoMessage)
    {
      return Ok();
    }
  }
}