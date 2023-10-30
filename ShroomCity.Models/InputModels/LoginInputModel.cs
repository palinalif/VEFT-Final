using System.ComponentModel.DataAnnotations;

namespace ShroomCity.Models.InputModels;

public class LoginInputModel
{
    [Required]
    public string EmailAddress { get; set; } = "";
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = "";
}