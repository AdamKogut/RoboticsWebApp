namespace Backend.Messages.UserManagement
{
  public class GetInviteInfoMessage
  {
    public string EncryptedString { get; set; }

    public GetInviteInfoMessage()
    {
      EncryptedString = string.Empty;
    }
  }
}