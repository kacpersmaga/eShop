namespace eShop.Shared.Interfaces.Security;

public interface ITokenRevocationService
{
    Task RevokeTokenByJtiAsync(string jti);
    Task<bool> IsTokenJtiRevokedAsync(string jti);
    Task RevokeAllTokensAsync();
}