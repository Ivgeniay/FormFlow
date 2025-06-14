using FormFlow.Domain.Models.Auth;

namespace FormFlow.Domain.Interfaces.Services
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo> GetUserInfoAsync(string code);
    }
}
