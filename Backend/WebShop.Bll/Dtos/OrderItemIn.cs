namespace WebShop.Bll.Dtos;

public record OrderItemIn
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
