using Microsoft.EntityFrameworkCore; 
  
using CityInfo.API.Data;
using CityInfo.API.Data.Entities;

namespace CityInfo.API.Services;

public class CityInfoRepository : ICityInfoRepository
{
    private readonly AppDbContext _appDbContext;

    public CityInfoRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _appDbContext.Cities.ToListAsync();
    }

    public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
    {
        if (includePointsOfInterest)
        {
            return await _appDbContext.Cities
                .Include(c => c.PointOfInterests)
                .FirstOrDefaultAsync(c => c.Id == cityId);
        }
        
        return await _appDbContext.Cities
            .FirstOrDefaultAsync(c => c.Id == cityId);
    }

    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await _appDbContext.Cities.AnyAsync(c => c.Id == cityId);
    }

    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
    {
        return await _appDbContext.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
    }

    public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
    {
        return await _appDbContext.PointOfInterests
            .FirstOrDefaultAsync(p => p.Id == pointOfInterestId && p.CityId == cityId);
    }

    public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);
        if (city is not null)
        {
            city.PointOfInterests.Add(pointOfInterest);
        }
    }
    
    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        _appDbContext.PointOfInterests.Remove(pointOfInterest);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _appDbContext.SaveChangesAsync() >= 0);
    }
}