using System.Security.Claims;

namespace Identity.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetMemberId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new UnauthorizedAccessException("User ID claim not found");
    }
}