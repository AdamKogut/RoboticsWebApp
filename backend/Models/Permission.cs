using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
  public class Permission
  {
    [Key]
    public Guid Id { get; set; }
    public int Permissions { get; set; }
    public Guid TeamId { get; set; }
    public Team Team { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Permission()
    {
      Team = new Team();
      User = new User();
    }
  }
}