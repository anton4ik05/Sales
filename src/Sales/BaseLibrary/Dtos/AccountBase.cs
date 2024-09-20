using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Dtos;

public class AccountBase
{
    [DataType(DataType.Text)]
    [Required]
    public string UserName { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; } = null!;
}