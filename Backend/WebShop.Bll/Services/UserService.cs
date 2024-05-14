using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WebShop.Bll.Dtos;
using WebShop.Bll.Exceptions;
using WebShop.Bll.Interfaces;
using WebShop.Dal;

namespace WebShop.Bll.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> GetUserCountAsync()
    {
        var userCount = await _context
            .Users
            .CountAsync();
        return userCount;
    }

    public async Task<UserOut> GetUserAsync(int userId)
    {
        return await _context.Users
            .ProjectTo<UserOut>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(u => u.Id == userId)
            ?? throw new EntityNotFoundException("A keresett felhasználó nem található");
    }

    public async Task<UserOut> FindUserAsync(UserIn user)
    {
        return await _context.Users
            .Where(u => u.Email == user.Email && u.Password == user.Password)
            .ProjectTo<UserOut>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync()
            ?? throw new EntityNotFoundException("A keresett felhasználó nem található");
    }

    public async Task<UserOut> InsertUserAsync(UserIn user)
    {
        var efUser = new Dal.Entities.User(user.Name!, user.Email, user.Password);
        await _context.Users.AddAsync(efUser);
        await _context.SaveChangesAsync();
        return await GetUserAsync(efUser.Id);
    }
}
