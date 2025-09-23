using CSDLVanHoaDuLichSonLa.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Reflection;

namespace CSDLVanHoaDuLichSonLa.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Đăng ký MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.RegisterMediaR.AssemblyMarker).Assembly));

            services.AddTransient<IJsonService, JsonService>();

            // Đăng ký DbContext với SQL Server
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            // CORS
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowAnyOrigin();
                });
            });

            return services;
        }
    }
}
