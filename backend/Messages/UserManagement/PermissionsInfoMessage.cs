namespace Backend.Messages.UserManagement
{
  public class PermissionsInfoMessage
  {
    public Guid UserId { get; set; }
    public Guid TeamId { get; set; }
    public List<string> Permissions { get; set; }

    public PermissionsInfoMessage()
    {
      Permissions = new List<string>();
    }
  }
}