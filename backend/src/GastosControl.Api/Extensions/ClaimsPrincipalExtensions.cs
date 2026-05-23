using System.Security.Claims;
using GastosControl.Application.Common;

namespace GastosControl.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var rawUserId = principal.FindFirstValue("sub")
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(rawUserId, out var userId))
        {
            throw new UnauthorizedAppException("Token de usuario invalido.");
        }

        return userId;
    }
}
