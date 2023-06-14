using Backend;
using Backend.Adapters;
using Backend.ImageUploadModule;
using Backend.Implementations;
using Backend.InventoryModule;
using Backend.Model;
using Backend.ProductCatalogModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseIISIntegration();
IConfiguration config = builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();
builder.Services.AddSingleton(config);
// TODO
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddDbContext<BackendDbContext>(options => options.UseSqlServer(config.GetConnectionString("DBConnectionString")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// When implementing Auth, uncomment this line:
app.MapGet("/", [Authorize] (ClaimsPrincipal user) => $"Hello World! {user.Identity!.Name}");
//app.MapGet("/", () => "Hello World!");

AzureADAuthorizationService authService = new(new Dictionary<string, IEnumerable<Permission>>
{
    ["Admin"] = new[] { Permission.GetProduct, Permission.ListProducts, Permission.ListProductsWithFilters, Permission.CreateProduct, Permission.UpdateProduct, Permission.AddStock, Permission.RemoveStock, Permission.UploadImage },
    ["User"] = new[] { Permission.GetProduct, Permission.ListProducts, Permission.ListProductsWithFilters }
});

new ProductCatalogModule()
    // TODO replace with AD Auth
    .AddModule(new AuthorizationAdapters(authService.Authorize))
    .ToList()
    .ForEach(endpoint => app.MapMethods(endpoint.Path, new[] { endpoint.Method.Method }, endpoint.Handler));

new InventoryModule()
    // TODO replace with AD Auth
    .AddModule(new AuthorizationAdapters(authService.Authorize))
    .ToList()
    .ForEach(endpoint => app.MapMethods(endpoint.Path, new[] { endpoint.Method.Method }, endpoint.Handler));

new ImageUploadModule()
    // TODO replace with AD Auth and Blob Storage implementation
    .AddModule(new AuthorizationAdapters(authService.Authorize), new ImageBlobStorageService(config))
    .ToList()
    .ForEach(endpoint => app.MapMethods(endpoint.Path, new[] { endpoint.Method.Method }, endpoint.Handler));

app.Run();
