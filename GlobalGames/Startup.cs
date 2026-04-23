using GlobalGames.Data;
using GlobalGames.Data.Entities;
using GlobalGames.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Cryptography;

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
            // 1. O DbContext DEVE ser registado primeiro (Dependência base)
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            // 2. Identity registado depois, consumindo o DbContext
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                
                // Política de Passwords Estrita (Nível de Produção)
                cfg.Password.RequiredLength = 10;
                cfg.Password.RequireDigit = true;
                cfg.Password.RequireUppercase = true;
                cfg.Password.RequireLowercase = true;
                cfg.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<DataContext>();

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

            // MIDDLEWARE DE SEGURANÇA (Posição Crítica: Antes de qualquer processamento dinâmico ou estático)
            app.Use(async (context, next) =>
            {
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

                context.Response.Headers["Content-Security-Policy"] = csp;

                await next();
            });

            // Ficheiros estáticos protegidos com os cabeçalhos acima
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Correção da rota por defeito: action agora aponta para 'Index'
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}