using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;
using BowlingCoreMVC.Services;
using Microsoft.AspNetCore.Rewrite;

namespace BowlingCoreMVC
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
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("MyDbConnection")));
                //execute database migration
                services.BuildServiceProvider().GetService<ApplicationDbContext>().Database.Migrate();
            }
            else
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["DefaultConnection"]));

            // dotnet 2.1+
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Session (only for passing Message around for now)
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession(options => 
            {
                options.Cookie.Name = "BowlingHub.Session";
                options.IdleTimeout = TimeSpan.FromHours(4);
            });

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection")));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration["DefaultConnection"]));

            //services.AddIdentity<IdentityUser, IdentityRole>(config =>
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            //    .AddRazorPagesOptions(options =>
            //    {
            //        //options.AllowAreas = true;
            //        options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            //        options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            //    });
            services.AddMvc().AddRazorRuntimeCompilation();

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(300);
                
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

                options.SlidingExpiration = true;
            });

            // Add application services.
            services.AddSingleton<IEmailSender, EmailSender>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                //TEMPORARY
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();

                //DEFAULT
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();

                // https?
                //var options = new RewriteOptions().AddRedirectToHttpsPermanent();
                //app.UseRewriter(options);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            // 2.2:
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
