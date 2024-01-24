using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.DataTransferObjects.Auth;

public class RegisterModel
{
    [Required, StringLength(100)]
    public string? FirstName { get; set; }

    [Required, StringLength(100)]
    public string? LastName { get; set; }

    [Required, StringLength(50)]
    public string? Username { get; set; }

    [Required, StringLength(128)]
    public string? Email { get; set; }

    [Required, StringLength(265)]
    public string? Password { get; set; }
}
