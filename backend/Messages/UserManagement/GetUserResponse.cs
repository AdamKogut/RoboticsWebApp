namespace Backend.Messages.UserManagement
{
  public class GetUserResponse
  {
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Dictionary<Guid, string> TeamDictionary { get; set; }

    public GetUserResponse()
    {
      FirstName = string.Empty;
      LastName = string.Empty;
      Email = string.Empty;
      TeamDictionary = new Dictionary<Guid, string>();
    }
  }
}