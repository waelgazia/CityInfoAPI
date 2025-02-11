using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityInfo.API.Data.Entities.Config;

public class PointOfInterestConfiguration : IEntityTypeConfiguration<PointOfInterest>
{
    public void Configure(EntityTypeBuilder<PointOfInterest> pointOfInterestBuilder)
    {
        pointOfInterestBuilder.ToTable("PointOfInterests");
        pointOfInterestBuilder.HasKey(p => p.Id);

        pointOfInterestBuilder.Property(p => p.Name)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(50)
            .IsRequired();
        pointOfInterestBuilder.Property(p => p.Description)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(200);

        pointOfInterestBuilder.HasOne(p => p.City)
            .WithMany(c => c.PointOfInterests)
            .HasForeignKey(p => p.CityId)
            .IsRequired();

        pointOfInterestBuilder.HasData(LoadPointOfInterests());
    }

    public List<PointOfInterest> LoadPointOfInterests()
    {
        return
        [
            new PointOfInterest()
            {
                Id = 1,
                Name = "Central Park",
                Description = "The most visited urban park in the United States.",
                CityId = 1
            },
            new PointOfInterest()
            {
                Id = 2,
                Name = "Empire State Building",
                Description = "A 102-story skyscraper located in Midtown Manhattan.",
                CityId = 1
            },
            new PointOfInterest()
            {
                Id = 3,
                Name = "Cathedral of Our Lady",
                Description = "A Gothic style cathedral, conceived by architects Jan and Piete.",
                CityId = 2
            },
            new PointOfInterest()
            {
                Id = 4,
                Name = "Antwerp Central Station",
                Description = "The finest example of railway architecture in Belgium.",
                CityId = 2
            },
            new PointOfInterest()
            {
                Id = 5,
                Name = "Eiffel Tower",
                Description = "A wrought iron lattice tower on the Champ de Mars, named after the architect Eiffel.",
                CityId = 3
            },
            new PointOfInterest()
            {
                Id = 6,
                Name = "The Louvre",
                Description = "The world's largest museum.",
                CityId = 3
            }
        ];
    }
}