using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

using CityInfo.API.Data;
using CityInfo.API.Models;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities/{cityId}/[controller]")]
public class PointOfInterestsController : ControllerBase
{
    private readonly CitiesDataStore _citiesDataStore;
    private readonly ILogger<PointOfInterestsController> _logger;
    private readonly IMailService _mailService;

    public PointOfInterestsController(CitiesDataStore citiesDataStore,
        ILogger<PointOfInterestsController> logger, IMailService mailService)
    {
        _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetCityPointOfInterests([FromRoute] int cityId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
            return NotFound("No city is found with the given city id");
        }

        return Ok(city.PointOfInterests);
    }

    [HttpGet("{pointOfInterestId}", Name = nameof(GetCityPointOfInterest))]
    public ActionResult<PointOfInterestDto> GetCityPointOfInterest([FromRoute] int cityId,
        [FromRoute] int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToFind = city.PointOfInterests
            .FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestToFind == null)
        {
            return NotFound();
        }

        return Ok(pointOfInterestToFind);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        [FromRoute] int cityId,
        [FromBody] PointOfInterestForCreationDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        int maxPointOfInterestId = _citiesDataStore.Cities
            .SelectMany(c => c.PointOfInterests)
            .Max(p => p.Id);

        PointOfInterestDto pointOfInterestToAdd = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };

        city.PointOfInterests.Add(pointOfInterestToAdd);

        // allows returning a response with a Location header
        return CreatedAtRoute(
            nameof(GetCityPointOfInterest), // route used for in location header
            new
            {
                cityId = cityId, pointOfInterestId = pointOfInterestToAdd.Id
            }, // route params for GetCityPointOfInterest
            pointOfInterestToAdd); // the object to include in the response body
    }

    [HttpPut("{pointOfInterestId}")]
    public ActionResult UpdatePointOfInterest(
        [FromRoute] int cityId,
        [FromRoute] int pointOfInterestId,
        [FromBody] PointOfInterestForUpdateDto pointOfInterest)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdate = city.PointOfInterests
            .FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestToUpdate == null)
        {
            return NotFound();
        }

        pointOfInterestToUpdate.Name = pointOfInterest.Name;
        pointOfInterestToUpdate.Description = pointOfInterest.Description;

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public ActionResult PartiallyUpdatePointOfInterest(
        [FromRoute] int cityId, 
        [FromRoute] int pointOfInterestId,
        [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdate = city.PointOfInterests
            .FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestToUpdate == null)
        {
            return NotFound();
        }

        var pointOfInterestToUpdateDto = new PointOfInterestForUpdateDto()
        {
            Name = pointOfInterestToUpdate.Name,
            Description = pointOfInterestToUpdate.Description,
        };
        
        patchDocument.ApplyTo(pointOfInterestToUpdateDto, ModelState);
        if (!ModelState.IsValid || !TryValidateModel(pointOfInterestToUpdateDto))
        {
            return BadRequest(ModelState);
        }
        
        pointOfInterestToUpdate.Name = pointOfInterestToUpdateDto.Name;
        pointOfInterestToUpdate.Description = pointOfInterestToUpdateDto.Description;

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestToDelete = city.PointOfInterests
            .FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestToDelete == null)
        {
            return NotFound();
        }

        city.PointOfInterests.Remove(pointOfInterestToDelete);
        _mailService.Send("Point of interest deleted.", 
            $"Point of interest {pointOfInterestToDelete.Name} with id {pointOfInterestToDelete.Id} was deleted!");
        
        return NoContent();
    }
}