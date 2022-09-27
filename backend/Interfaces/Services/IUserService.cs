using Backend.Enums;
using Backend.Models;

namespace Backend.Interfaces.Services
{
  public interface IUserService
  {
    bool CreateUser(string firstName, string lastName, string email, string password, out string reason);
    string Authenticate(string email, string password);
    bool DeleteUser(Guid userId, out string reason);
    bool ChangePassword(Guid userId, string password, out string reason);
    bool ModifyUser(Guid userId, string firstName, string lastName, string email, out string reason);
  }
}