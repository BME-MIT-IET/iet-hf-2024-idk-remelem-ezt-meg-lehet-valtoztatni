namespace WebShop.Bll.Dtos;

public record Category
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
