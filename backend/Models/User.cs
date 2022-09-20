using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
  public class User
  {
    [Key]
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string password { get; set; }
    public int NumberFailures { get; set; }
    public DateTime LastFailure { get; set; }
    public List<Permission> Permissions { get; set; }
  }
}