using CityInfo.API.Data.Entities;

namespace CityInfo.API.Services;

public interface ICityInfoRepository
{
    // city controller
    Task<IEnumerable<City>> GetCitiesAsync();
    Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(
        string? name, string? searchQuery, int pageNumber, int pageSize);
    Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
    Task<bool> CityExistsAsync(int cityId);

    // point of interest controller
    Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
    Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId);
    Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
    void DeletePointOfInterest(PointOfInterest pointOfInterest);
    Task<bool> SaveChangesAsync();
}