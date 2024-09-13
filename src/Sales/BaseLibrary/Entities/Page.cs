using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities;

[Table("pages")]
public class Page
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(256)]
    public string Uri { get; set; } = null!;

    [MaxLength(256)]
    public string? Text { get; set; }

    public string Promo { get; set; } = null!;

    [JsonIgnore]
    [InverseProperty(nameof(Product.Page))]
    public ICollection<Product> Products { get; set; }

    private DateTime _updatedAt;
    private DateTime _expirationAt;
    
    [Required]
    public DateTime UpdatedAt
    {
        get => _updatedAt;
        set => _updatedAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    [Required]
    public DateTime ExpirationAt 
    {
        get => _expirationAt;
        set => _expirationAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

}