using Backend.Enums;

namespace Backend.Messages.UserManagement
{
  public class InviteUserMessage
  {
    public Guid TeamId { get; set; }
    public string Email { get; set; }
    public List<PermissionEnum> Permissions { get; set; }

    public InviteUserMessage()
    {
      Email = "";
      Permissions = new List<PermissionEnum>();
    }
  }
}