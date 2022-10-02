namespace Backend.Messages.UserManagement
{
  public class CreateUserMessage
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public CreateUserMessage()
    {
      FirstName = "";
      LastName = "";
      Email = "";
      Password = "";
    }
  }
}