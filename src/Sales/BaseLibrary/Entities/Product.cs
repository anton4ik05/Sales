using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities;

[Table("products")]
public class Product
{
    [Key] public int Id { get; set; }

    [Required] [MaxLength(128)] public string ProductName { get; set; } = null!;

    [MaxLength(64)] public string? ProductCode { get; set; }

    [Required] public string Link { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal DiscountPrice { get; set; }
    public int PageId { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(PageId))]
    public Page Page { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    [InverseProperty(nameof(Image.Product))]
    public ICollection<Image>? Images { get; set; }
}