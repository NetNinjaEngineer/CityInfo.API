using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.DataTransferObjects.Auth;

public class TokenRequestModel
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}
