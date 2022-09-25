namespace Backend.Messages.UserManagement
{
  public class NewTeamMessage
  {
    public Guid UserId { get; set; }
    public string Name { get; set; }

    public NewTeamMessage()
    {
      Name = "";
    }
  }
}