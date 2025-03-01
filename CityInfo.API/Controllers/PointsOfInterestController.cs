using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

using AutoMapper;

using CityInfo.API.Models;
using CityInfo.API.Services;
using CityInfo.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace CityInfo.API.Controllers;

[Authorize] /* to enforce authentication to access the controller */
[ApiController]
[Route("api/cities/{cityId}/[controller]")]
public class PointsOfInterestController : ControllerBase
{
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;

    public PointsOfInterestController(
        ICityInfoRepository cityInfoRepository,
        IMapper mapper,
        ILogger<PointsOfInterestController> logger,
        IMailService mailService)
    {
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetCityPointsOfInterest([FromRoute] int cityId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
            return NotFound("No city is found with the given city id");
        }

        var pointsOfInterestEntities = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestEntities));
    }

    [HttpGet("{pointOfInterestId}", Name = nameof(GetCityPointOfInterest))]
    public async Task<ActionResult<PointOfInterestDto>> GetCityPointOfInterest([FromRoute] int cityId,
        [FromRoute] int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterestEntity));
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
        [FromRoute] int cityId,
        [FromBody] PointOfInterestForCreationDto pointOfInterest)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = _mapper.Map<PointOfInterest>(pointOfInterest);

        await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();

        var pointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(pointOfInterestEntity);

        // allows returning a response with a Location header
        return CreatedAtRoute(
            nameof(GetCityPointOfInterest), // route used for in location header
            new
            {
                cityId = cityId, pointOfInterestId = pointOfInterestToReturn.Id
            }, // route params for GetCityPointOfInterest
            pointOfInterestToReturn); // the object to include in the response body
    }

    [HttpPut("{pointOfInterestId}")]
    public async Task<ActionResult> UpdatePointOfInterest(
        [FromRoute] int cityId,
        [FromRoute] int pointOfInterestId,
        [FromBody] PointOfInterestForUpdateDto pointOfInterest)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        // this overload will override the values from the destination (pointOfInterestEntity)
        // with those from the source object (pointOfInterest) 
        _mapper.Map(pointOfInterest, pointOfInterestEntity);

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public async Task<ActionResult> PartiallyUpdatePointOfInterest(
        [FromRoute] int cityId,
        [FromRoute] int pointOfInterestId,
        [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }
        
        var pointOfInterestToUpdateEntity = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
        if (pointOfInterestToUpdateEntity is null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdateDto = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestToUpdateEntity);
        
        patchDocument.ApplyTo(pointOfInterestToUpdateDto, ModelState);
        if (!ModelState.IsValid || !TryValidateModel(pointOfInterestToUpdateDto))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(pointOfInterestToUpdateDto, pointOfInterestToUpdateEntity);
        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestToDelete = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
        if (pointOfInterestToDelete is null)
        {
            return NotFound();
        }

        _cityInfoRepository.DeletePointOfInterest(pointOfInterestToDelete);
        await _cityInfoRepository.SaveChangesAsync();
        
        _mailService.Send("Point of interest deleted.",
            $"Point of interest {pointOfInterestToDelete.Name} with id {pointOfInterestToDelete.Id} was deleted!");

        return NoContent();
    }
}