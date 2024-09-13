using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Dtos;

public class AccountBase
{
    [DataType(DataType.EmailAddress)]
    [Required]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; } = null!;
}