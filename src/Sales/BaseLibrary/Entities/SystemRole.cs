using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities;

[Table("system_roles")]
public class SystemRole
{
    [Key]
    public int Id { get; set; }
    
    public string? Name { get; set; }
}