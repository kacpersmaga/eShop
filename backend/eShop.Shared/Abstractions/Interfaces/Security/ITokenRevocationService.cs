namespace eShop.Shared.Abstractions.Interfaces.Security;

public interface ITokenRevocationService
{
    Task RevokeTokenByJtiAsync(string jti);
    Task<bool> IsTokenJtiRevokedAsync(string jti);
    Task RevokeAllTokensAsync();
}