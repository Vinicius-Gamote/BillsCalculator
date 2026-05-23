namespace GastosControl.Application.Abstractions.Security;

public sealed class JwtSettings
{
    public string Issuer { get; set; } = "GastosControl";

    public string Audience { get; set; } = "GastosControl.Client";

    public string Secret { get; set; } = "change-this-development-secret-32-chars";

    public int ExpirationMinutes { get; set; } = 120;
}
