namespace eShop.Services.Interfaces;

public interface ITokenRevocationService
{
    Task RevokeTokenByJtiAsync(string jti);
    Task<bool> IsTokenJtiRevokedAsync(string jti);
    Task RevokeAllTokensAsync();
}