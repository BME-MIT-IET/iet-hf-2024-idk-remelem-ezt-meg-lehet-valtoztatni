using WebShop.Dal.Entities;

namespace WebShop.Bll.Dtos;

public record OrderIn
{
    public OrderStatus OrderStatus { get; set; }
    public List<OrderItemIn> OrderItems { get; set; } = new List<OrderItemIn>();
}
