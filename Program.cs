using KIM_Style.Data;
using KIM_Style.Models;
using KIM_Style.Models.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options => 
    {
        options.DefaultScheme = "KimStyleCookie"; // Establece el esquema predeterminado
        options.DefaultSignInScheme = "KimStyleCookie"; // Establece el esquema para inicio de sesión

    })
        .AddCookie("KimStyleCookie", options =>
        {
            options.LoginPath = "/Home/Index"; // Página de inicio de sesión
            options.AccessDeniedPath = "/Home/Index"; // Página de acceso denegado
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequiereLogin", policy => policy.RequireAuthenticatedUser());
});


builder.Services.AddDbContext<KIM_StyleContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection") ?? throw new InvalidOperationException("Connection string 'SqlServerConnection' not found.")));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;


    try
    {
        var dbContext = services.GetRequiredService<KIM_StyleContext>();
       // dbContext.Database.EnsureDeleted();
       // dbContext.Database.EnsureCreated();
        SeedData.Initialize(services);

    }
    catch (Exception ex)
    {
        Console.WriteLine("Error manipulando el seed data " + ex.Message);
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
