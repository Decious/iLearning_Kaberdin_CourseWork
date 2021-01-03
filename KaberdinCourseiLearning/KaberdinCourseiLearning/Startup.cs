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
            services.AddDefaultIdentity<CustomUser>(options => {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.POLICY_ADMIN, policy =>
                    policy.RequireRole(RoleNames.ROLE_ADMINISTRATOR));
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager)
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            IdentityInitializer.SeedData(userManager, roleManager);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

    }
}
