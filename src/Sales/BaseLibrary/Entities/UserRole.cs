using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities;

[Table("user_roles")]
public class UserRole(ApplicationUser user, SystemRole role)
{
    public int Id { get; set; }
    
    public int RoleId { get; set; }
    
    public int UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = user;

    [ForeignKey(nameof(RoleId))]
    public SystemRole Role { get; set; } = role;
}