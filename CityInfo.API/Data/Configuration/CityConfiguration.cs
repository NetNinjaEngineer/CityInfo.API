using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityInfo.API.Data.Configuration;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .HasColumnType("VARCHAR").HasMaxLength(100).IsRequired();

        builder.Property(p => p.Country)
            .HasColumnType("VARCHAR").HasMaxLength(100).IsRequired();

        builder.HasData(SeedData.GetCities());
        builder.ToTable("Cities");
    }
}
