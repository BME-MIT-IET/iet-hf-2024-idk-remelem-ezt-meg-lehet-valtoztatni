using Microsoft.EntityFrameworkCore;
using WebShop.Bll.Dtos;

namespace WebShop.Bll.Interfaces;

public interface IOrderService
{
    public Task<int> GetOrderCountAsync();
    public Task<IEnumerable<OrderOut>> GetOrdersAsync();
    public Task<OrderOut> GetOrderAsync(int orderId);
    public Task<IEnumerable<OrderOut>> GetUserOrdersAsync(int userId);
    public Task<OrderOut> InsertOrderAsync(int userId, OrderIn order);
    public Task UpdateOrderAsync(int id, OrderIn order);
}
