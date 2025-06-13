using FormFlow.Domain.Models.General;
using FormFlow.Infrastructure.Models;
using FormFlow.WebApi.Middlewares;

namespace FormFlow.WebApi.Common.Extensions
{
    public static class HttpContextExtensions
    {
        public static CurrentUser? GetCurrentUser(this HttpContext context) =>
            context.Items["CurrentUser"] as CurrentUser;

        public static Guid? GetCurrentUserId(this HttpContext context) =>
            context.GetCurrentUser()?.Id;

        public static bool IsUserInRole(this HttpContext context, UserRole role) =>
            context.GetCurrentUser()?.HasRole(role) ?? false;

        public static bool IsCurrentUser(this HttpContext context, Guid userId) =>
            context.GetCurrentUserId() == userId;
    }
}
