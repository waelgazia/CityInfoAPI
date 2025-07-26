using AutoMapper;

namespace CityInfo.API.Profiles;

public class PointOfInterestProfile : Profile
{
    public PointOfInterestProfile()
    {
        CreateMap<Data.Entities.PointOfInterest, Models.PointOfInterestDto>().ReverseMap();
        CreateMap<Models.PointOfInterestForCreationDto, Data.Entities.PointOfInterest>();
        CreateMap<Models.PointOfInterestForUpdateDto, Data.Entities.PointOfInterest>().ReverseMap();
    }
}