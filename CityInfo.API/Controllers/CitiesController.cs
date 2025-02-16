using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly int maxCitiesPageSize = 20;
    
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities(
        [FromQuery] string? name,
        [FromQuery] string? searchQuery,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize > maxCitiesPageSize) pageSize = maxCitiesPageSize;
        
        var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(
            name, searchQuery, pageNumber, pageSize);
        
        // to add pagination metadata in a response header called X-Pagination
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        
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