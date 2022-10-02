namespace Backend.Messages.UserManagement
{
  public class RemoveUserMessage
  {
    public Guid UserId { get; set; }
    public Guid TeamId { get; set; }
    public Guid RequesterId { get; set; }
  }
}