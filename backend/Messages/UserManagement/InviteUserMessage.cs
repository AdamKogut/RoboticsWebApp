namespace Backend.Messages.UserManagement
{
  public class InviteUserMessage
  {
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }

    public InviteUserMessage()
    {
      Email = "";
    }
  }
}