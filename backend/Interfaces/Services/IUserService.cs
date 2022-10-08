using Backend.Models;

namespace Backend.Interfaces.Services
{
  public interface IUserService
  {
    string Authenticate(string email, string password);
    string ChangePassword(string email);
    string CreateUser(string firstName, string lastName, string email, string password);
    string DeleteUser(Guid userId);
    User GetUserInfo(Guid userInfo);
    string ModifyUser(Guid userId, string firstName, string lastName, string email, string password = "");
  }
}