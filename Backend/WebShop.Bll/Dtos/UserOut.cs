namespace WebShop.Bll.Dtos;

public record UserOut
{
    public int Id { get; init; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; } = false;
    public ICollection<OrderOut> Orders { get; set; } = new List<OrderOut>();
}
