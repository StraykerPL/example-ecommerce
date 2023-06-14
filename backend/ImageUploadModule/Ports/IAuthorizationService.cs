using System.Security.Claims;

namespace Backend.ImageUploadModule;

public interface IAuthorizationService
{
    public void Authorize(string permission, ClaimsPrincipal user);
}