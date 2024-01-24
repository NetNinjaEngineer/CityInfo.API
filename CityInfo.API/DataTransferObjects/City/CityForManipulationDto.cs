using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.DataTransferObjects.City;

//[CityForManipulation]
public abstract class CityForManipulationDto
{
    [Required(ErrorMessage = "You should fill out the name.")]
    [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 characters.")]
    public string? Name { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Country { get; set; }
    public int Population { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
