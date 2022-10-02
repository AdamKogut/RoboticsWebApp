using Backend.Enums;

namespace Backend.Messages.UserManagement
{
  public class GetUserResponse
  {
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public GetUserResponse()
    {
      FirstName = string.Empty;
      LastName = string.Empty;
      Email = string.Empty;
    }
  }
}