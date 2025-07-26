using CityInfo.API.Profiles;
using CityInfo.API.Data.Entities;

namespace CityInfo.API.Models;

public class CityWithoutPointsOfInterestDto : IMapFrom<City>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}