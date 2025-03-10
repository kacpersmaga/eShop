using eShop.Infrastructure.Data;
using eShop.Shared.Interfaces.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Implementations
{
    public class TokenRevocationService(
        ApplicationDbContext dbContext,
        IDistributedCache cache,
        ILogger<TokenRevocationService> logger)
        : ITokenRevocationService
    {

        public async Task RevokeTokenByJtiAsync(string jti)
        {
            try
            {
                await cache.SetStringAsync(
                    $"revoked_jti:{jti}",
                    DateTime.UtcNow.ToString("O"),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                    });
                
                logger.LogInformation("Revoked token with jti: {Jti}", jti);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to revoke token jti: {Jti}", jti);
                throw;
            }
        }

        public async Task<bool> IsTokenJtiRevokedAsync(string jti)
        {
            var revoked = await cache.GetStringAsync($"revoked_jti:{jti}");
            return revoked != null;
        }

        public async Task RevokeAllTokensAsync()
        {
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    UPDATE AspNetUsers 
                    SET RefreshToken = NULL, 
                        RefreshTokenExpiryTime = '2000-01-01'");
                
                var currentVersion = await cache.GetStringAsync("global_token_version") ?? "0";
                var newVersion = (int.Parse(currentVersion) + 1).ToString();

                await cache.SetStringAsync(
                    "global_token_version",
                    newVersion,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365)
                    });

                logger.LogWarning("Revoked ALL tokens system-wide. New token version: {Version}", newVersion);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to revoke all tokens");
                throw;
            }
        }
    }
}
