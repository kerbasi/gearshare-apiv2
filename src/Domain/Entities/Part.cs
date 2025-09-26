using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GearShare.Domain.Entities;

public class Part
{
    public int PartId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Sku { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0")]
    public int StockQuantity { get; set; } = 0;
    
    [Required]
    public int ManufacturerId { get; set; }
    
    // Navigation properties
    public Manufacturer Manufacturer { get; set; } = null!;
    public ICollection<PartCompatibility> PartCompatibilities { get; set; } = new List<PartCompatibility>();
}
