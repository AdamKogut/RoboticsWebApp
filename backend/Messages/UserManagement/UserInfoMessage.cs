namespace Backend.Messages.UserManagement
{
  public class UserInfoMessage
  {
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public UserInfoMessage()
    {
      FirstName = "";
      LastName = "";
      Email = "";
      Password = "";
    }
  }
}