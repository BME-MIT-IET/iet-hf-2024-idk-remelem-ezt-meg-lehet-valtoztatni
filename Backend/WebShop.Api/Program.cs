using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using WebShop.Bll.Dtos;
using WebShop.Bll.Exceptions;
using WebShop.Bll.Interfaces;
using WebShop.Bll.Services;
using WebShop.Dal;

namespace WebShop.Api;

public partial class Program
{
    const string allowSelfPolicy = nameof(allowSelfPolicy);
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Console.WriteLine(builder.Configuration["ConnectionStrings:DefaultConnection"]);
        builder.Services.AddDbContext<AppDbContext>(o =>
            o.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"])
        );

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddControllers().AddJsonOptions(o =>
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
        );
        builder.Services.AddProblemDetails(options =>
        {
            options.IncludeExceptionDetails = (ctx, ex) => false;
            options.Map<EntityNotFoundException>(
                (ctx, ex) =>
                {
                    var pd = StatusCodeProblemDetails.Create(StatusCodes.Status404NotFound);
                    pd.Title = ex.Message;
                    return pd;
                }
            );
            options.Map<FieldIsRequiredException>(
                (ctx, ex) =>
                {
                    var pd = StatusCodeProblemDetails.Create(StatusCodes.Status400BadRequest);
                    pd.Title = ex.Message;
                    return pd;
                }
            );
        });
        builder.Services.AddCors(o =>
            o.AddPolicy(allowSelfPolicy, builder =>
                builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            )
        );
        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();
        builder.Services.AddAuthorization(builder =>
        {
            builder
                .AddPolicy("Admin", builder =>
                    builder.RequireClaim(ClaimTypes.Role, "Admin")
                );
            builder
                .AddPolicy("User", builder =>
                    builder.RequireClaim(ClaimTypes.Role, "User")
                );
        });

        builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
        builder.Services.AddAutoMapper(typeof(WebApiProfile));
        builder.Services.AddTransient<IProductService, ProductService>();
        builder.Services.AddTransient<IOrderService, OrderService>();
        builder.Services.AddTransient<ICategoryService, CategoryService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddHostedService<TimedStatLoggerSerivce>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.ToString());

            var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
            options.IncludeXmlComments(filePath);
        });

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(allowSelfPolicy);
        app.UseProblemDetails();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHealthChecks("/health");
        app.MapControllers();
        app.Run();
    }
}