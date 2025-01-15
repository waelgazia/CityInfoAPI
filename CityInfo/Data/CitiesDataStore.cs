﻿using CityInfo.Models;

namespace CityInfo.Data;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; private set; } = [];
    public static CitiesDataStore Current { get; } = new CitiesDataStore();
    
    private CitiesDataStore()
    {
        LoadCities();
    }

    private void LoadCities()
    {
        Cities =
        [
            new CityDto()
            {
                Id = 1,
                Name = "New York City",
                Description = "The one with that big park.",
                PointOfInterests = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Central Park",
                        Description = "The most visited urban park in the United States."
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2,
                        Name = "Empire State Building",
                        Description = "A 102-story skyscraper located in Midtown Manhattan."
                    }
                }
            },

            new CityDto()
            {
                Id = 2,
                Name = "Antwerp",
                Description = "The one with the cathedral that was never really finished",
                PointOfInterests = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 3,
                        Name = "Cathedral of Our Lady",
                        Description = "A Gothic style cathedral, conceived by architects Jan and Piete."
                    },
                    new PointOfInterestDto()
                    {
                        Id = 4,
                        Name = "Antwerp Central Station",
                        Description = "The finest example of railway architecture in Belgium."
                    }
                }
            },

            new CityDto()
            {
                Id = 3,
                Name = "Paris",
                Description = "The one with that pig tower.",
                PointOfInterests = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 5,
                        Name = "Eiffel Tower",
                        Description = "A wrought iron lattice tower on the Champ de Mars, named after the architect Eiffel."
                    },
                    new PointOfInterestDto()
                    {
                        Id = 6,
                        Name = "The Louvre",
                        Description = "The world's largest museum."
                    }
                }
            }
        ];
    }
}