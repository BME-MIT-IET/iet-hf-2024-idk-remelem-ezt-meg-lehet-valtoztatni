using System.Security.Claims;

namespace WebShop.Api.Controllers;

public static class ClaimsHelper
{
    public static string GetUserRole(ClaimsPrincipal user)
    {
        var userRole = user.Claims.Single(c => c.Type == ClaimTypes.Role).Value;
        return userRole;
    }

    public static int GetUserId(ClaimsPrincipal user)
    {
        var userId = int.Parse(user.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
        return userId;
    }

    public static bool IsAdmin(ClaimsPrincipal user)
    {
        return GetUserRole(user) == "Admin";
    }
}
