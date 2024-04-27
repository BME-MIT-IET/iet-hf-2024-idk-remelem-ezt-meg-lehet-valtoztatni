namespace WebShop.Bll.Dtos;

public record ProductOut
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Price { get; init; }
    public string? Description { get; init; }
    public int CategoryId { get; init; }
    public Category Category { get; init; } = null!;
}
