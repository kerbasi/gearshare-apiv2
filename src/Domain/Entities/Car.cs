using System.ComponentModel.DataAnnotations;

namespace GearShare.Domain.Entities;

public class Car
{
    public int CarId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Make { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;
    
    [Required]
    public int YearStart { get; set; }
    
    [Required]
    public int YearEnd { get; set; }
    
    // Navigation properties
    public ICollection<PartCompatibility> PartCompatibilities { get; set; } = new List<PartCompatibility>();
}
