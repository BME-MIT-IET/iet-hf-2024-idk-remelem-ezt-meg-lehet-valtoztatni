using WebShop.Dal.Entities;

namespace WebShop.Bll.Dtos;

public record OrderOut
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ICollection<OrderItemOut> OrderItems { get; set; } = new List<OrderItemOut>();
}
