using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities;

public class ApplicationUser
{
    [Key]
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}