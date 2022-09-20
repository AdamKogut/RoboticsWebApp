using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
  public class Team
  {
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Permission> Permissions { get; set; }
  }
}