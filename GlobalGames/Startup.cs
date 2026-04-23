using GlobalGames.Data;
using GlobalGames.Data.Entities;
using GlobalGames.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http; // Adicionado para uso de Headers
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
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

            // --- INÍCIO DO MIDDLEWARE DE SEGURANÇA (CSP & NONCE) ---
            app.Use(async (context, next) =>
            {
                // Gera o Nonce único para a requisição atual
                var nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                
                // Disponibiliza o Nonce para o _Layout.cshtml via HttpContext
                context.Items["CspNonce"] = nonce;

                // Define a política de segurança no cabeçalho da resposta
                var csp = $"default-src 'self'; " +
                          $"script-src 'self' 'nonce-{nonce}'; " +
                          $"style-src 'self' 'nonce-{nonce}'; " +
                          $"img-src 'self' data:; " +
                          $"font-src 'self' data:; " +
                          $"frame-ancestors 'self'; " +
                          $"object-src 'none'; " +
                          $"base-uri 'self';";

                context.Response.Headers.Add("Content-Security-Policy", csp);

                await next();
            });
            // --- FIM DO MIDDLEWARE DE SEGURANÇA ---

            app.UseRouting();

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