using AutoMapper;

namespace CityInfo.API.Profiles;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<Data.Entities.City, Models.CityDto>();
        CreateMap<Data.Entities.City, Models.CityWithoutPointsOfInterestDto>();
    }
}