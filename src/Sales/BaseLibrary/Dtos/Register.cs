using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Dtos;

public class Register : AccountBase
{
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    [Required]
    public string ConfirmPassword { get; set; } = null!;
}