using Microsoft.AspNetCore.Mvc.Filters;
using FormFlow.Infrastructure.Models;
using FormFlow.Domain.Models.General;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.WebApi.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole _requiredRole;
        private readonly bool _requireExactRole;

        public RequireRoleAttribute(UserRole requiredRole, bool requireExactRole = false)
        {
            _requiredRole = requiredRole;
            _requireExactRole = requireExactRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Authentication required" });
                return;
            }

            var currentUser = context.HttpContext.Items[Infrastructure.Constants.Auth.CURRENT_USER_KEY] as CurrentUser;
            if (currentUser == null)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid user context" });
                return;
            }

            if (currentUser.IsBlocked)
            {
                context.Result = new ForbidResult();
                return;
            }

            bool hasPermission = _requireExactRole
                ? currentUser.Role == _requiredRole
                : currentUser.Role.HasFlag(_requiredRole);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class RequireOwnerOrAdminAttribute : Attribute, IAuthorizationFilter
        {
            private readonly string _userIdParameterName;

            public RequireOwnerOrAdminAttribute(string userIdParameterName = "userId")
            {
                _userIdParameterName = userIdParameterName;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (!context.HttpContext.User.Identity?.IsAuthenticated == true)
                {
                    context.Result = new UnauthorizedObjectResult(new { message = "Authentication required" });
                    return;
                }

                var currentUser = context.HttpContext.Items[Infrastructure.Constants.Auth.CURRENT_USER_KEY] as CurrentUser;
                if (currentUser == null)
                {
                    context.Result = new UnauthorizedObjectResult(new { message = "Invalid user context" });
                    return;
                }

                if (currentUser.IsBlocked)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                if (currentUser.HasRole(UserRole.Admin))
                {
                    return;
                }

                if (context.RouteData.Values.TryGetValue(_userIdParameterName, out var userIdValue) &&
                    Guid.TryParse(userIdValue?.ToString(), out var targetUserId))
                {
                    if (currentUser.Id == targetUserId)
                    {
                        return;
                    }
                }

                if (context.HttpContext.Request.Query.TryGetValue(_userIdParameterName, out var queryUserId) &&
                    Guid.TryParse(queryUserId, out var queryTargetUserId))
                {
                    if (currentUser.Id == queryTargetUserId)
                    {
                        return;
                    }
                }

                context.Result = new ForbidResult();
            }
        }
    }

}
