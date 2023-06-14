using Backend.Model;
using System.Security.Claims;

namespace Backend.Implementations
{
    public class AzureADAuthorizationService
    {
        private readonly IDictionary<string, IEnumerable<Permission>> _rolePermissions;

        public AzureADAuthorizationService(IDictionary<string, IEnumerable<Permission>> rolePermissions)
        {
            _rolePermissions = rolePermissions;
        }

        public void Authorize(Permission permission, ClaimsPrincipal user)
        {
            bool doesntHaveRole = !_rolePermissions.TryGetValue(user.FindFirstValue(ClaimTypes.Role) ?? "User", out IEnumerable<Permission>? permissions);
            if (doesntHaveRole || permissions?.Contains(permission) == false)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
