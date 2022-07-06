namespace Identity_Server.Models;

internal sealed class TokenData
{
    public string Token { get; init; }
    public string RefreshToken { get; init; }
    public DateTime RefreshTokenExpirationDate { get; init; }
}