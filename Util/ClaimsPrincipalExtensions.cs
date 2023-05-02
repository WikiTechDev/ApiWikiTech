using System.Security.Claims;

namespace ApiWikiTech.Util
{

    public static class ClaimsPrincipalExtensions
    {
        public static string? GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

}
