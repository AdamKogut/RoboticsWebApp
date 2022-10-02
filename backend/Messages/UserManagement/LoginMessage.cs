namespace Backend.Messages.UserManagement
{
  public class LoginMessage
  {
    public string Email { get; set; }
    public string Password { get; set; }

    public LoginMessage()
    {
      Email = "";
      Password = "";
    }
  }
}