using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityInfo.API.Data.Entities.Config;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> cityBuilder)
    {
        cityBuilder.ToTable("Cities");
        cityBuilder.HasKey(c => c.Id);

        cityBuilder.Property(c => c.Name)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(50)
            .IsRequired();
        cityBuilder.Property(c => c.Description)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(200);

        cityBuilder.HasData(LoadCities());
    }

    private List<City> LoadCities()
    {
        return
        [
            new City()
            {
                Id = 1,
                Name = "New York City",
                Description = "The one with that big park.",
            },

            new City()
            {
                Id = 2,
                Name = "Antwerp",
                Description = "The one with the cathedral that was never really finished",
            },

            new City()
            {
                Id = 3,
                Name = "Paris",
                Description = "The one with that pig tower.",
            }
        ];
    }
}