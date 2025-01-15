using System.ComponentModel.DataAnnotations;

namespace CityInfo.Models;

public class PointOfInterestForUpdateDto
{
    [MaxLength(50)]
    [Required(ErrorMessage = "You should provide a name value!")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
}