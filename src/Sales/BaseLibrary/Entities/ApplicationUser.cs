using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities;

[Table("users")]
public class ApplicationUser
{
    [Key]
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}