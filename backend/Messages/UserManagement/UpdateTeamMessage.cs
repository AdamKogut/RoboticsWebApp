namespace Backend.Messages.UserManagement
{
  public class UpdateTeamMessage
  {
    public Guid TeamId { get; set; }
    public string Name { get; set; }

    public UpdateTeamMessage()
    {
      Name = string.Empty;
    }
  }
}