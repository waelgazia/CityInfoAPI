using System.ComponentModel.DataAnnotations;

using CityInfo.API.Profiles;
using CityInfo.API.Data.Entities;

namespace CityInfo.API.Models;

public class PointOfInterestForUpdateDto : IMapFrom<PointOfInterest>
{
    [MaxLength(50)]
    [Required(ErrorMessage = "You should provide a name value!")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
}