using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities;

[Table("user_roles")]
public class UserRole()
{
    public int Id { get; set; }
    
    public int RoleId { get; set; }
    
    public int UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }

    [ForeignKey(nameof(RoleId))]
    public SystemRole Role { get; set; }
}