using Microsoft.AspNetCore.Mvc;

using AutoMapper;

using CityInfo.API.Models;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities()
    {
        var cityEntities = await _cityInfoRepository.GetCitiesAsync();
        return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> /* as the return type is not certain */ GetCity(
        [FromRoute] int id, 
        [FromQuery] bool includePointsOfInterest = false)
    {
        var cityToReturn = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
        if (cityToReturn is null) return NotFound();

        return includePointsOfInterest
            ? Ok(_mapper.Map<CityDto>(cityToReturn))
            : Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(cityToReturn));
    }
} 