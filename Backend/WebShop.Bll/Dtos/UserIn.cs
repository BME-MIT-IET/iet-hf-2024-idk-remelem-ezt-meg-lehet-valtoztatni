using System.ComponentModel.DataAnnotations;

namespace WebShop.Bll.Dtos;

public record UserIn
{
    [MinLength(3, ErrorMessage = "Túl rövid név")]
    [MaxLength(64, ErrorMessage = "A név maximum 64 karakter hosszú lehet")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Az email megadása kötelező")]
    [EmailAddress(ErrorMessage = "Nem email formátumú")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A jelszó megadása kötelező")]
    [MinLength(8, ErrorMessage = "Túl rövid jelszó")]
    [MaxLength(64, ErrorMessage = "A név maximum 64 karakter hosszú lehet")]
    public string Password { get; set; }
}
