using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using KaberdinCourseiLearning.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using KaberdinCourseiLearning.Data.Models;
using KaberdinCourseiLearning.Helpers;
using KaberdinCourseiLearning.Managers;
using KaberdinCourseiLearning.Middleware;
using KaberdinCourseiLearning.Hubs;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using KaberdinCourseiLearning.Areas.Identity;

namespace KaberdinCourseiLearning
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<CustomUser>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<CustomUserManager>();
            services.AddControllers();
            services.AddRazorPages().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.POLICY_ADMIN, policy => 
                    policy.RequireRole(RoleNames.ROLE_ADMINISTRATOR));
                options.AddPolicy(PolicyNames.POLICY_AUTHENTICATED, policy =>
                    policy.RequireAuthenticatedUser());
            });
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = Environment.GetEnvironmentVariable("AuthFacebookID");
                options.AppSecret = Environment.GetEnvironmentVariable("AuthFacebookSecret");
            });
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = Environment.GetEnvironmentVariable("AuthGoogleID");
                options.ClientSecret = Environment.GetEnvironmentVariable("AuthGoogleSecret");
            });
            services.AddCors();
            services.AddSignalR();
            services.AddScoped<ImageManager>();
            services.AddScoped<CollectionManager>();
            services.AddScoped<ProductManager>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CustomUserManager userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });
            if (Environment.GetEnvironmentVariable("PORT") == null)
            {
                app.UseHttpsRedirection();
            }
            else
            {
                app.UseCustomHttpsRedirection();
            }
            app.UseStaticFiles();
            app.UseRequestLocalization(GetLocalizationOptions());
            app.UseRouting();
            app.UseAuthentication();
            IdentityInitializer.SeedData(userManager, roleManager);
            app.UseAuthorization();
            app.UseUserValidationMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHub<ProductHub>("/Item/ProductHub");
            });
        }
        private RequestLocalizationOptions GetLocalizationOptions()
        {
            var supportedCultures = Configuration.GetSection("Localization").GetChildren().ToDictionary(x => x.Key, x => x.Value).Keys.ToArray();
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            return localizationOptions;
        }
    }
}
