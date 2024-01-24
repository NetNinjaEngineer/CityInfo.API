using CityInfo.API.DataTransferObjects.City;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.ValidationAttributes;

public class CityForManipulationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var cityForManipulationDto = (CityForManipulationDto)validationContext.ObjectInstance;

        if (cityForManipulationDto.Name == cityForManipulationDto.Country)
            return new ValidationResult("The provided country must be different from name", ["CityForManipulationDto"]);

        return ValidationResult.Success;
    }
}
