using Backend.Enums;

namespace Backend.Messages.UserManagement
{
  public class ChangePasswordMessage
  {
    public string Email { get; set; }

    public ChangePasswordMessage()
    {
      Email = string.Empty;
    }
  }
}