using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WebShop.Bll.Dtos;
using WebShop.Bll.Exceptions;
using WebShop.Bll.Interfaces;
using WebShop.Dal;
using WebShop.Dal.Entities;

namespace WebShop.Bll.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public OrderService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> GetOrderCountAsync()
    {
        var orderCount = await _context
            .Orders
            .CountAsync();
        return orderCount;
    }

    public async Task<IEnumerable<OrderOut>> GetOrdersAsync()
    {
        var orders = await _context.Orders
            .ProjectTo<OrderOut>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return orders;
    }

    public async Task<IEnumerable<OrderOut>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .ProjectTo<OrderOut>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<OrderOut> GetOrderAsync(int orderId)
    {
        var order = await _context.Orders
            .ProjectTo<OrderOut>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(o => o.Id == orderId)
            ?? throw new EntityNotFoundException("Nem található a rendelés");
        return order;
    }

    public async Task<OrderOut> InsertOrderAsync(int userId, OrderIn order)
    {
        var efOrder = _mapper.Map<Order>(order);
        efOrder.OrderDate = DateTime.Now;
        efOrder.OrderStatus = OrderStatus.Unread;
        efOrder.UserId = userId;
        await _context.Orders.AddAsync(efOrder);
        await _context.SaveChangesAsync();
        return await GetOrderAsync(efOrder.Id);
    }

    public async Task UpdateOrderAsync(int id, OrderIn order)
    {
        var efProduct = await _context
            .Orders
            .Where(o => o.Id == id)
            .SingleOrDefaultAsync()
            ?? throw new EntityNotFoundException("A termék nem található");
        efProduct.OrderStatus = order.OrderStatus;
        efProduct.OrderItems = order.OrderItems.Select(oi => _mapper.Map<OrderItem>(oi)).ToList();
        _context.SaveChanges();
    }
}
