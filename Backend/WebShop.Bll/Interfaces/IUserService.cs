using WebShop.Bll.Dtos;

namespace WebShop.Bll.Interfaces;

public interface IUserService
{
    public Task<int> GetUserCountAsync();
    public Task<UserOut> GetUserAsync(int id);
    public Task<UserOut> FindUserAsync(UserIn user);
    public Task<UserOut> InsertUserAsync(UserIn user);
}
