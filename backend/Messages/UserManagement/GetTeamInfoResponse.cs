namespace Backend.Messages.UserManagement
{
  public class GetTeamInfoResponse
  {
    public string Name { get; set; }
    public List<UserTeamObject> Users { get; set; }

    public GetTeamInfoResponse()
    {
      Name = string.Empty;
      Users = new List<UserTeamObject>();
    }
  }
}