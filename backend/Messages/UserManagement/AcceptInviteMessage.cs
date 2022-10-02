using Backend.Enums;

namespace Backend.Messages.UserManagement
{
  public class AcceptInviteMessage
  {
    public Guid TeamId { get; set; }
    public List<PermissionEnum> Permissions { get; set; }

    public AcceptInviteMessage()
    {
      Permissions = new List<PermissionEnum>();
    }
  }
}