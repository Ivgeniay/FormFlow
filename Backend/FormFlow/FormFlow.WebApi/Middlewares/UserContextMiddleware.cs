using FormFlow.Domain.Models.General;
using FormFlow.Infrastructure.Models;
using FormFlow.Domain.Models.Auth;
using FormFlow.WebApi.Common;
using System.Security.Claims;
using FormFlow.Domain.Interfaces.Services.Jwt;

namespace FormFlow.WebApi.Middlewares
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                    var userNameClaim = context.User.FindFirst(ClaimTypes.Name);
                    var emailClaim = context.User.FindFirst(ClaimTypes.Email);
                    var roleClaim = context.User.FindFirst(ClaimTypes.Role);
                    var authTypeClaim = context.User.FindFirst(Infrastructure.Constants.JwtClaimNames.AuthType);
                    var isBlockedClaim = context.User.FindFirst(Infrastructure.Constants.JwtClaimNames.IsBlocked);

                    if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                    {
                        var currentUser = new CurrentUser
                        {
                            Id = userId,
                            UserName = userNameClaim?.Value ?? string.Empty,
                            Email = emailClaim?.Value ?? string.Empty,
                            Role = Enum.TryParse<UserRole>(roleClaim?.Value, out var role) ? role : UserRole.User,
                            AuthType = Enum.TryParse<AuthType>(authTypeClaim?.Value, out var authType) ? authType : AuthType.Internal,
                            IsBlocked = bool.TryParse(isBlockedClaim?.Value, out var blocked) && blocked,
                            IsAuthenticated = true
                        };

                        if (currentUser.IsBlocked)
                        {
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsync(Constants.Erros_Msg.USER_BLOCKED);
                            return;
                        }

                        context.Items[Infrastructure.Constants.Auth.CURRENT_USER_KEY] = currentUser;
                    }
                }
                catch
                {
                    context.Items[Infrastructure.Constants.Auth.CURRENT_USER_KEY] = null;
                }
            }

            await _next(context);
        }
    }
}
