using Backend.Enums;
using Backend.Models;

namespace Backend.Interfaces.Services
{
  public interface IUserService
  {
    string CreateUser(string firstName, string lastName, string email, string password);
    string Authenticate(string email, string password);
    string DeleteUser(Guid userId);
    string ChangePassword(string email);
    string ModifyUser(Guid userId, string firstName, string lastName, string email, string password);
    User GetUserInfo(Guid userInfo);
  }
}