using CityInfo.API.Profiles;
using CityInfo.API.Data.Entities;

namespace CityInfo.API.Models;

public class PointOfInterestDto : IMapFrom<PointOfInterest>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}