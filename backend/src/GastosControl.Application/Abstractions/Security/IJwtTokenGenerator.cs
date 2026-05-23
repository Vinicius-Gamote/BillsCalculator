using GastosControl.Domain.Entities;

namespace GastosControl.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    TokenResult CreateToken(User user);
}

public sealed record TokenResult(string AccessToken, DateTimeOffset ExpiresAt);
