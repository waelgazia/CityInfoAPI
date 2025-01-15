using Microsoft.AspNetCore.Mvc;

using CityInfo.Data;
using CityInfo.Models;

namespace CityInfo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<CityDto>> GetCities()
    {
        return Ok(CitiesDataStore.Current.Cities);
    }

    [HttpGet("{id}")]
    public ActionResult<CityDto> GetCity([FromRoute] int id)
    {
        var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
        return cityToReturn is not null ? Ok(cityToReturn) : NotFound();
    }
} 