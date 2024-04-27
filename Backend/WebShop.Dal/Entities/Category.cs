using System.ComponentModel.DataAnnotations;

namespace WebShop.Dal.Entities;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A katergória név megadása kötelező")]
    public string Name { get; set; }
    public ICollection<Product> Items { get; set; } = new List<Product>();

    public Category(string name)
    {
        Name = name;
    }
}
