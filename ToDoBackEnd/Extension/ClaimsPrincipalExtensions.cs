using System.Security.Claims;

namespace ToDoBackEnd.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return id == null
            ? throw new Exception("UserId not found in token")
            : int.Parse(id);
    }
}