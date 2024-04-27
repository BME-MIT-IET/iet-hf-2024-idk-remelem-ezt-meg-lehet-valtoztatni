namespace WebShop.Bll.Dtos;

public record OrderItemOut
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public ProductOut Product { get; set; } = null!;
    public int Quantity { get; set; }
}
