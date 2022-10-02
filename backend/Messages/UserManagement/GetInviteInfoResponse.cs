using Backend.Enums;

namespace Backend.Messages.UserManagement
{
  public class GetInviteInfoResponse
  {
    public string Email { get; set; }
    public List<PermissionEnum> Permissions { get; set; }
    public Guid TeamId { get; set; }

    public GetInviteInfoResponse()
    {
      Email = string.Empty;
      Permissions = new List<PermissionEnum>();
    }
  }
}