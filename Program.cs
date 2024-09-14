using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using OrdersManagement.Data.IdentityContext;
using OrdersManagement.Data.OrderContext;
using OrdersManagement.Models;
using OrdersManagement.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var configuration = builder.Configuration;

// Configure identity services
var identityConnection = configuration.GetConnectionString("IdentityConnection");

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(identityConnection));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityDbContext>();

// Configure order management services
var orderContextConnectionString = configuration.GetConnectionString("OrderContext");
builder.Services.AddDbContext<OrderManagementContext>(options =>
    options.UseSqlServer(orderContextConnectionString));

var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Orders.json");
if (!File.Exists(jsonFilePath))
{
    // Create the file if it doesn't exist
    File.Create(jsonFilePath).Close();
}

// Register the SQL and JSON repositories
builder.Services.AddScoped<SqlOrderRepository>();
builder.Services.AddScoped<JsonOrderRepository>(provider => new JsonOrderRepository(jsonFilePath));

// Register CombinedOrderRepository with dependencies
builder.Services.AddScoped<IOrderRepository, CombinedOrderRepository>(provider =>
{
    var sqlOrderRepository = provider.GetRequiredService<SqlOrderRepository>();
    var jsonOrderRepository = provider.GetRequiredService<JsonOrderRepository>();
    return new CombinedOrderRepository(sqlOrderRepository, jsonOrderRepository);
});

// Apply migrations
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    identityDbContext.Database.Migrate();

    var orderManagementContext = scope.ServiceProvider.GetRequiredService<OrderManagementContext>();
    orderManagementContext.Database.Migrate();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve static files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "",
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream",
    OnPrepareResponse = ctx =>
    {
        var headers = ctx.Context.Response.Headers;
        var contentType = headers["Content-Type"].ToString();
        if (contentType.Contains("text/css") || contentType.Contains("application/javascript"))
        {
            var minifiedPath = ctx.Context.Request.Path.Value.Replace(".js", ".min.js").Replace(".css", ".min.css");
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", minifiedPath)))
            {
                ctx.Context.Request.Path = minifiedPath;
            }
        }
    }
});

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
