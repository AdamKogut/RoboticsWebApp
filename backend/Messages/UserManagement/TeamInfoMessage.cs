namespace Backend.Messages.UserManagement
{
  public class TeamInfoMessage
  {
    public Guid TeamId { get; set; }
    public string Name { get; set; }

    public TeamInfoMessage()
    {
      Name = "";
    }
  }
}