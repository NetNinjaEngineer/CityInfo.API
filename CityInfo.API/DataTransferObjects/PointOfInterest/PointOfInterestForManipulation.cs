using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.DataTransferObjects.PointOfInterest;

public abstract record PointOfInterestForManipulation
{
    [Required]
    [MaxLength(50, ErrorMessage = "Maximum Length for Name is 50 characters")]
    [MinLength(20, ErrorMessage = "Minimum Length for Name is 20 characters")]
    public string? Name { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "Maximum Length for Category is 50 characters")]
    public string? Category { get; set; }

    [Required]
    [MaxLength(200, ErrorMessage = "Maximum Length for Description is 200 characters")]
    public string? Description { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
}