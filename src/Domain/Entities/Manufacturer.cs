using System.ComponentModel.DataAnnotations;

namespace GearShare.Domain.Entities;

public class Manufacturer
{
    public int ManufacturerId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Country { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Part> Parts { get; set; } = new List<Part>();
}
