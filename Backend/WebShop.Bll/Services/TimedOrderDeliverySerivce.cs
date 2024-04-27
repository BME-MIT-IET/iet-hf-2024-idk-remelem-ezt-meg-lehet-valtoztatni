using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebShop.Bll.Interfaces;

namespace WebShop.Bll.Services;

public class TimedStatLoggerSerivce : IHostedService, IDisposable
{
    private IServiceScopeFactory _serviceScopeFactory;
    private Timer? _timer = null;
    private readonly ILogger<TimedStatLoggerSerivce> _logger;

    public TimedStatLoggerSerivce(IServiceScopeFactory serviceScopeFactory, ILogger<TimedStatLoggerSerivce> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    private async void LogStats(object? state)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            try
            {
                var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                _logger.Log(LogLevel.Information, "{0}: CategoryCount: {1}", DateTime.Now, await categoryService.GetCategoryCountAsync());
                _logger.Log(LogLevel.Information, "{0}: OrderCount: {1}", DateTime.Now, await orderService.GetOrderCountAsync());
                _logger.Log(LogLevel.Information, "{0}: ProductCount: {1}", DateTime.Now, await productService.GetProductCountAsync());
                _logger.Log(LogLevel.Information, "{0}: UserCount: {1}", DateTime.Now, await userService.GetUserCountAsync());
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Unable to report stats");
                //await Console.Out.WriteLineAsync(ex.ToString());
            }
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(LogStats, null, 0, 30*1000);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
