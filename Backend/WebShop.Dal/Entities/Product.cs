using System.ComponentModel.DataAnnotations;

namespace WebShop.Dal.Entities;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A termék név megadása kötelező")]
    public string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A termék ára nem lehet nullánál kevesebb")]
    public int Price { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "A termék kategória megadása kötelező")]
    public Category Category { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
    public ICollection<Order> Orders { get; } = new List<Order>();

    public Product(string name, int price)
    {
        Name = name;
        Price = price;
    }
}
