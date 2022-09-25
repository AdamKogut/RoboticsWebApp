using Backend.Interfaces.Repository;

namespace Backend.Services
{
  public class UserService
  {
    private readonly IUserManagementRepository _userManagementRepository;

    public UserService(IUserManagementRepository userManagementRepository)
    {
      _userManagementRepository = userManagementRepository;
    }

    public string Authenticate(string email, string password)
    {

    }
  }
}