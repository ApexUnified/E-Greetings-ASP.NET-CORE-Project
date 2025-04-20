using Microsoft.AspNetCore.Authorization;

namespace E_Greetings.Authorization
{
    using System.Security.Claims;
    using E_Greetings.Models;
    using Microsoft.AspNetCore.Authorization;

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly EGreetingsContext _dbContext;

        public PermissionAuthorizationHandler(EGreetingsContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userRole = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            

            if (userRole == null)
            {
                context.Fail();
                return;
            }

            var hasPermission = _dbContext.RolePermissions
                .Any(rp => rp.Role.Name == userRole && rp.Permission.Name == requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }

}
