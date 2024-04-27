using System.ComponentModel.DataAnnotations;

namespace WebShop.Dal.Entities;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A név megadása kötelező")]
    [MinLength(3, ErrorMessage = "Túl rövid név")]
    [MaxLength(64, ErrorMessage = "A név maximum 64 karakter hosszú lehet")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Az email megadása kötelező")]
    [EmailAddress(ErrorMessage = "Nem email formátumú")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A jelszó megadása kötelező")]
    [MinLength(8, ErrorMessage = "Túl rövid jelszó")]
    [MaxLength(64, ErrorMessage = "A név maximum 64 karakter hosszú lehet")]
    public string Password { get; set; }

    public bool IsAdmin { get; set; } = false;
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
}
