using Application;
using CSDLVanHoaDuLichSonLa.Extensions;
using CSDLVanHoaDuLichSonLa.Models;
using CSDLVanHoaDuLichSonLa.Services;
using Domain;
using Domain.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Application
builder.Services.AddApplicationServices(builder.Configuration);

// Identity config
builder.Services.AddIdentity<AppUser, AppRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
})
.AddRoles<AppRole>()
.AddEntityFrameworkStores<DataContext>()
.AddErrorDescriber<CustomIdentityErrorDescriber>()
.AddDefaultTokenProviders();

// Cookie hạn 1 giờ
builder.Services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "Huecit.Cookie";
    config.LoginPath = "/home/login";
    config.ExpireTimeSpan = TimeSpan.FromHours(1);
    config.SlidingExpiration = true;
});
// Đăng ký factory custom claims
builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomClaimsPrincipalFactory>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        // hoặc policy.WithOrigins("http://localhost:5098").AllowAnyHeader().AllowAnyMethod();
    });
});


builder.Configuration.AddJsonFile("nav.json", optional: true, reloadOnChange: true);
builder.Services.Configure<AdminNavModel>(builder.Configuration.GetSection("Navs"));

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ICapTinhService, CapTinhService>();
builder.Services.AddTransient<ICapXaService, CapXaService>();
builder.Services.AddTransient<INoiDungDaNguServices, NoiDungDaNguServices>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Thêm thông tin API
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API cho hệ thống của tôi"
    });

    // API Key
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "API Key Authentication",
        Name = "x-api-key",
        Type = SecuritySchemeType.ApiKey,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new List<string>()
        }
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger";
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map routes
app.MapControllers();

// Routes
app.MapAreaControllerRoute(
    name: "AdminTool",
    areaName: "AdminTool",
    pattern: "AdminTool/{controller=Home}/{action=Index}"
);

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Login}/{id?}"
    pattern: "{controller=Home}/{action=Login}/{id?}"
    //defaults: new { controller = "Home", action = "Index" }
);

//app.MapControllerRoute(
//    name: "Home/Index",
//    pattern: "AdminTool/{controller=Home}/{action=Index}"
//);

// Migration + Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        await context.Database.MigrateAsync();
        await Seed.SeedData(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, ex.Message);
    }
}

app.Run();
