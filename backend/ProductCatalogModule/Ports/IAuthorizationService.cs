using System.Security.Claims;

namespace Backend.ProductCatalogModule;

public interface IAuthorizationService
{
    void Authorize(string permission, ClaimsPrincipal user);
}