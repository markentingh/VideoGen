using VideoGen.Data.Entities.Auth;

namespace VideoGen.Data.Interfaces.Auth
{
    public interface IAppUserTokenRepository
    {
        Task<AppUserTokens> Add(AppUserTokens token);
        Task<AppUserTokens> FindByTokenIP(string token, string ip);
        Task<bool> IsTokenUnique(string token);
        Task ExtendRefreshToken(AppUserTokens token);
    }
}
