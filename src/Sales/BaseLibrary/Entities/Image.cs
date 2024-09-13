using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities;

[Table("images")]
public class Image
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [MaxLength(255)]
    public string Name { get; set; }
    
    [Required]
    public byte[] Data { get; set; }
    
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
}