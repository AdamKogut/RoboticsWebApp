namespace Backend.Messages.UserManagement
{
  public class PermissionsInfoMessage
  {
    public Guid TeamId { get; set; }
    public List<UserTeamObject> Users { get; set; }

    public PermissionsInfoMessage()
    {
      Users = new List<UserTeamObject>();
    }
  }
}