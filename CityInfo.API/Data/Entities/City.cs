namespace CityInfo.API.Data.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<PointOfInterest> PointOfInterests { get; set; } = new List<PointOfInterest>();
}