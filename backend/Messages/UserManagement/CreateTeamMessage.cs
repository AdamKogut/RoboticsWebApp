namespace Backend.Messages.UserManagement
{
  public class CreateTeamMessage
  {
    public string Name { get; set; }

    public CreateTeamMessage()
    {
      Name = "";
    }
  }
}