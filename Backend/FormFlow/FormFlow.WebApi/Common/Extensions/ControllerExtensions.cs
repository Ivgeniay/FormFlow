using FormFlow.Domain.Models.General;
using FormFlow.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Common.Extensions
{
    public static class ControllerExtensions
    {
        public static CurrentUser? GetCurrentUser(this ControllerBase controller) =>
            controller.HttpContext.Items[Infrastructure.Constants.Auth.CURRENT_USER_KEY] as CurrentUser;
        
        public static Guid? GetCurrentUserId(this ControllerBase controller) =>
            controller.GetCurrentUser()?.Id;
        
        public static bool IsCurrentUserInRole(this ControllerBase controller, UserRole role) =>
            controller.GetCurrentUser()?.HasRole(role) ?? false;
    }
}
