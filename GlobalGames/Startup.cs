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
using System.Threading.Tasks;

namespace GlobalGames
{
    public class Startup
    {
        public Startup(IConfiguration configuration) { Configuration = configuration; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(cfg =>
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
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
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }

            app.UseHttpsRedirection();

            // Chamada modular do Middleware de Segurança
            app.Use(SecurityMiddleware());

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Método extraído para melhorar a testabilidade e organização
        private Func<RequestDelegate, RequestDelegate> SecurityMiddleware()
        {
            return next => async context =>
            {
                // Geração de Nonce Criptograficamente Seguro (RFC Compliance)
                string nonce;
                using (var rng = RandomNumberGenerator.Create())
                {
                    var nonceBytes = new byte[16];
                    rng.GetBytes(nonceBytes);
                    nonce = Convert.ToBase64String(nonceBytes);
                }

                context.Items["CspNonce"] = nonce;

                // CSP Hardened: Inclui proteção contra hijacking de formulários e exfiltração
                var csp = $"default-src 'self'; " +
                          $"script-src 'self' 'nonce-{nonce}'; " +
                          $"style-src 'self' 'nonce-{nonce}'; " +
                          $"img-src 'self' data:; " +
                          $"font-src 'self' data:; " +
                          $"connect-src 'self'; " + // Bloqueia chamadas AJAX externas não autorizadas
                          $"form-action 'self'; " +  // Impede envio de formulários para sites maliciosos
                          $"frame-src 'self'; " +    // Controla iFrames permitidos
                          $"frame-ancestors 'self'; " +
                          $"object-src 'none'; " +
                          $"base-uri 'self';";

                context.Response.Headers["Content-Security-Policy"] = csp;

                await next(context);
            };
        }
    }
}