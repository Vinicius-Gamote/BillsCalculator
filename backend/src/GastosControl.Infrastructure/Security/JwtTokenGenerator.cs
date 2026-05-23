using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GastosControl.Application.Abstractions.Security;
using GastosControl.Domain.Entities;

namespace GastosControl.Infrastructure.Security;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(JwtSettings settings)
    {
        _settings = settings;
    }

    public TokenResult CreateToken(User user)
    {
        if (string.IsNullOrWhiteSpace(_settings.Secret) || _settings.Secret.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Secret deve ter pelo menos 32 caracteres.");
        }

        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(_settings.ExpirationMinutes);

        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };

        var payload = new Dictionary<string, object>
        {
            ["sub"] = user.Id.ToString(),
            ["email"] = user.Email,
            ["name"] = user.Name,
            ["iss"] = _settings.Issuer,
            ["aud"] = _settings.Audience,
            ["iat"] = now.ToUnixTimeSeconds(),
            ["nbf"] = now.ToUnixTimeSeconds(),
            ["exp"] = expiresAt.ToUnixTimeSeconds(),
            ["jti"] = Guid.NewGuid().ToString()
        };

        var encodedHeader = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
        var encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
        var unsignedToken = $"{encodedHeader}.{encodedPayload}";
        var signature = Sign(unsignedToken);

        return new TokenResult($"{unsignedToken}.{signature}", expiresAt);
    }

    private string Sign(string unsignedToken)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_settings.Secret));
        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedToken));
        return Base64UrlEncode(signatureBytes);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
