using ManekiApp.Client;
using ManekiApp.Server.Components;
using ManekiApp.Server.Data;
using ManekiApp.Server.Models;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Radzen;
using _Imports = ManekiApp.Client._Imports;
using ManekiAppDBService = ManekiApp.Server.ManekiAppDBService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();
// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();
builder.Services.AddRadzenComponents();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ManekiAppDBService>();
builder.Services.AddDbContext<ManekiAppDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection"));
});
builder.Services.AddControllers().AddOData(opt =>
{
    var oDataBuilderManekiAppDB = new ODataConventionModelBuilder();
    oDataBuilderManekiAppDB.EntitySet<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>("AuthorPages");
    oDataBuilderManekiAppDB.EntitySet<ManekiApp.Server.Models.ManekiAppDB.Image>("Images");
    oDataBuilderManekiAppDB.EntitySet<ManekiApp.Server.Models.ManekiAppDB.Post>("Posts");
    oDataBuilderManekiAppDB.EntitySet<ManekiApp.Server.Models.ManekiAppDB.Subscription>("Subscriptions");
    oDataBuilderManekiAppDB.EntitySet<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>("UserSubscriptions");
    oDataBuilderManekiAppDB.EntitySet<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>("UserVerificationCodes");
    oDataBuilderManekiAppDB.EntitySet<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment>("UserChatPayments");
    oDataBuilderManekiAppDB.EntitySet<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification>("UserChatNotifications");
    opt.AddRouteComponents("odata/ManekiAppDB", oDataBuilderManekiAppDB.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
builder.Services.AddScoped<ManekiApp.Client.ManekiAppDBService>();
builder.Services.AddHttpClient("ManekiApp.Server").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false }).AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<SecurityService>();
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllers().AddOData(o =>
{
    var oDataBuilder = new ODataConventionModelBuilder();
    oDataBuilder.EntitySet<ApplicationUser>("ApplicationUsers");
    var usersType = oDataBuilder.StructuralTypes.First(x => x.ClrType == typeof(ApplicationUser));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Password)));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.ConfirmPassword)));
    oDataBuilder.EntitySet<ApplicationRole>("ApplicationRoles");
    o.AddRouteComponents("odata/Identity", oDataBuilder.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
builder.Services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();
builder.Services.AddScoped<ManekiAppDBService>();
builder.Services.AddDbContext<ManekiAppDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection"));
});
builder.Services.AddScoped<ManekiAppDBService>();
builder.Services.AddDbContext<ManekiAppDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection"));
});
builder.Services.AddScoped<ManekiApp.Server.ManekiAppDBService>();
builder.Services.AddDbContext<ManekiApp.Server.Data.ManekiAppDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection"));
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseHeaderPropagation();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveWebAssemblyRenderMode().AddAdditionalAssemblies(typeof(_Imports).Assembly);
app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().Database.Migrate();
using var scope = app.Services.CreateScope();
SeedData(scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>());
app.Run();
void SeedData(RoleManager<ApplicationRole> roleManager)
{
    var roles = new List<string>
    {
        "Admin",
        "FreeUser",
        "Author",
        "SubscriberTier1",
        "SubscriberTier2",
        "SubscriberTier3"
    };
    foreach (var role in roles)
    {
        if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new ApplicationRole(role)).GetAwaiter().GetResult();
        }
    }
}