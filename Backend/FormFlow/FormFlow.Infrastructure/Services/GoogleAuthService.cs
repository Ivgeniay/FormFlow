using FormFlow.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2.Flows;
using FormFlow.Domain.Models.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;

namespace FormFlow.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public GoogleAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _clientId = _configuration["GoogleOAuth:ClientId"] ?? throw new ArgumentNullException("GoogleOAuth:ClientId");
            _clientSecret = _configuration["GoogleOAuth:ClientSecret"] ?? throw new ArgumentNullException("GoogleOAuth:ClientSecret");
            _redirectUri = _configuration["GoogleOAuth:RedirectUri"] ?? throw new ArgumentNullException("GoogleOAuth:RedirectUri");
        }

        public async Task<GoogleUserInfo> GetUserInfoAsync(string code)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                Scopes = new[] { "openid", "email", "profile" }
            });

            var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                userId: "user_" + Guid.NewGuid().ToString(),
                code: code,
                redirectUri: _redirectUri,
                CancellationToken.None);

            var credential = new UserCredential(flow, "user", tokenResponse);

            var oauth2Service = new Oauth2Service(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "FormFlow"
            });

            var userInfo = await oauth2Service.Userinfo.Get().ExecuteAsync();

            return new GoogleUserInfo
            {
                GoogleId = userInfo.Id,
                Email = userInfo.Email,
                Name = userInfo.Name ?? userInfo.Email,
                Picture = userInfo.Picture,
            };
        }
    }
}
