using GlobalGames.Data;
using GlobalGames.Data.Entities;
using GlobalGames.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity; // Necessário para IdentityRole
using Microsoft.EntityFrameworkCore; // Necessário para UseSqlServer
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Cryptography; // Necessário para RandomNumberGenerator

namespace GlobalGames
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequiredLength = 10;
                // Outras configurações de password simplificadas conforme pedido
            }).AddEntityFrameworkStores<DataContext>();

            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<ISubscriberRepository, SubscriberRepository>();
            services.AddScoped<ILeadRepository, LeadRepository>();
            services.AddScoped<IConverterHelper, ConverterHelper>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // MIDDLEWARE DE SEGURANÇA CORRIGIDO
            app.Use(async (context, next) =>
            {
                // 1. Geração Criptograficamente Segura (16 bytes = 128 bits de entropia)
                var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
                
                context.Items["CspNonce"] = nonce;

                var csp = $"default-src 'self'; " +
                          $"script-src 'self' 'nonce-{nonce}'; " +
                          $"style-src 'self' 'nonce-{nonce}'; " +
                          $"img-src 'self' data:; " +
                          $"font-src 'self' data:; " +
                          $"frame-ancestors 'self'; " +
                          $"object-src 'none'; " +
                          $"base-uri 'self';";

                // 2. Uso de atribuição direta para evitar erros de duplicidade de header
                context.Response.Headers["Content-Security-Policy"] = csp;

                await next();
            });

            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Home}/{id?}");
            });
        }
    }
}