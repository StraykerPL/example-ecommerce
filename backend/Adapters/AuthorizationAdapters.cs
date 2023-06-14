using Backend.Model;
using System.Security.Claims;

namespace Backend.Adapters;

public class AuthorizationAdapters : ProductCatalogModule.IAuthorizationService, InventoryModule.IAuthorizationService, ImageUploadModule.IAuthorizationService
{
    private readonly Action<Permission, ClaimsPrincipal> _authorize;

    public AuthorizationAdapters(Action<Permission, ClaimsPrincipal> authorize)
    {
        _authorize = authorize;
    }

    public void Authorize(string permission, ClaimsPrincipal user)
    {
        switch (permission)
        {
            case ProductCatalogModule.Permissions.GetProduct:
                _authorize(Permission.GetProduct, user);
                break;

            case ProductCatalogModule.Permissions.ListProducts:
                _authorize(Permission.ListProducts, user);
                break;

            case ProductCatalogModule.Permissions.ListProductsWithFilters:
                _authorize(Permission.ListProductsWithFilters, user);
                break;

            case ProductCatalogModule.Permissions.CreateProduct:
                _authorize(Permission.CreateProduct, user);
                break;

            case ProductCatalogModule.Permissions.UpdateProduct:
                _authorize(Permission.UpdateProduct, user);
                break;

            case InventoryModule.Permissions.AddStock:
                _authorize(Permission.AddStock, user);
                break;

            case InventoryModule.Permissions.RemoveStock:
                _authorize(Permission.RemoveStock, user);
                break;

            case ImageUploadModule.Permissions.UploadImage:
                _authorize(Permission.UploadImage, user);
                break;

            default:
                throw new NotImplementedException();
        };
    }
}