using Backend.Enums;

namespace Backend.Messages.UserManagement
{
  public class UserTeamObject
  {
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<PermissionEnum> Permissions { get; set; }

    public UserTeamObject()
    {
      FirstName = string.Empty;
      LastName = string.Empty;
      Email = string.Empty;
      Permissions = new List<PermissionEnum>();
    }
  }
}