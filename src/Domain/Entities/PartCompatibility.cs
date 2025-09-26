namespace GearShare.Domain.Entities;

public class PartCompatibility
{
    public int PartId { get; set; }
    public int CarId { get; set; }
    
    // Navigation properties
    public Part Part { get; set; } = null!;
    public Car Car { get; set; } = null!;
}
