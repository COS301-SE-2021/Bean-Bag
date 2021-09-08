using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using BeanBag.Database;
using BeanBag.Services;

namespace BeanBag
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
            // Azure B2C OpenIdConnect Authentication service setup
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAdB2C"));

            services.AddRazorPages().AddMicrosoftIdentityUI();
            
            services.Configure<OpenIdConnectOptions>(
                OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Events.OnTokenValidated = async context =>
                    {
                        context.Properties.RedirectUri = "/Tenant";

                        await Task.FromResult(0);
                    };

                    options.Events.OnSignedOutCallbackRedirect = async context =>
                    {
                        context.Properties.RedirectUri = "/LandingPage";
                        
                        await Task.FromResult(0);
                    };

                });

            services.AddControllersWithViews();

            // Connecting to the sql server and to the specified DB using the app-settings.json ConnectionStrings defaultConnection contents
            services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(Configuration.GetValue<string>("Database:DefaultConnection"))
            );
            
            // Connecting to Tenant DB
            services.AddDbContext<TenantDbContext>(options => 
                options.UseSqlServer(Configuration.GetValue<string>("Database:TenantConnection"))
            );
            
        
            //Adding service classes to be used as a DI
            services.AddTransient<IInventoryService, InventoryService>();
            services.AddTransient<IItemService, ItemService>();
            services.AddTransient<IAiService, AiService>();
            services.AddTransient<IDashboardAnalyticsService, DashboardAnalyticsService>();
            services.AddTransient<IBlobStorageService, BlobStorageService>();

            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<ITenantService,TenantService>();
            services.AddTransient<ITenantBlobStorageService, TenantBlobStorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/LandingPage/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            } 
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=LandingPage}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
