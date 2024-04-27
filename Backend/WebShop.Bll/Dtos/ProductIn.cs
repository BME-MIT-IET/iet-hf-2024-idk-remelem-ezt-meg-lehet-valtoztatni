using System.ComponentModel.DataAnnotations;

namespace WebShop.Bll.Dtos;

public record ProductIn
{
    public string Name { get; init; } = string.Empty;
    public int Price { get; init; }
    public string? Description { get; init; }
    public int? CategoryId { get; init; }
}