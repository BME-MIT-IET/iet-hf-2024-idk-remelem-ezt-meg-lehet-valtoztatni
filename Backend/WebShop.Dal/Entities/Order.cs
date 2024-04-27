using System.ComponentModel.DataAnnotations;

namespace WebShop.Dal.Entities;

public class Order
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A rendelés dátuma megadása kötelező")]
    public DateTime OrderDate { get; set; }

    [Required(ErrorMessage = "A rendelés állapotának megadása kötelező")]
    public OrderStatus OrderStatus { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public int UserId { get; set; }
    public User User { get; set; }
}
