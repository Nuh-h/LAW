using ArabicLearning.Repositories;
using ArabicLearning.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Westwind.AspNetCore.LiveReload;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using ArabicLearning.Repositories.Models;
// using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Threading;

//add mysql 
// using MySQL.Data.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

//adding Okta auth
using Okta.AspNetCore;

namespace ArabicLearning
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
            services.AddHttpClient();
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();
            services.AddLiveReload();

            services.AddControllersWithViews();
            services.AddScoped<IImagesRepository, ImagesRepository>();
            services.AddScoped<ICoursesRepository, CoursesRepository>();

            #region EF CORE and IDENTITY
            //connecting EF to Database
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppIdentityDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            //registering identity services
            services.AddIdentity<AppIdentityUser, AppIdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>();
            #endregion

            #region Cookie-based Authentication
            // a default authentication (login/logout) against our db, options for external auth via OpenIdConnect

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) //When only cookie
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "defaultCookie";//"GoogleOpenID";//
            })
            .AddCookie("defaultCookie", options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/denied";

                //events in the cookie authentication pipeline
                options.Events = new CookieAuthenticationEvents()
                {
                    OnSigningIn = async context =>
                    {
                        //we can add roles here
                        var principal = context.Principal;
                        if (principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                        {
                            if (principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "N03")
                            {
                                var claimsIdentity = principal.Identity as ClaimsIdentity;
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                            }
                        }
                        await Task.CompletedTask;
                    },

                    OnSignedIn = async context =>
                    {
                        await Task.CompletedTask;
                    },

                    OnValidatePrincipal = async context =>
                    {
                        await Task.CompletedTask;
                    }
                };
            })
            .AddOpenIdConnect("GoogleOpenID", options =>
            {
                options.ClientId = Configuration["Authentication:GoogleOpenID:ClientId"];
                options.ClientSecret = Configuration["Authentication:GoogleOpenID:ClientSecret"];
                options.CallbackPath = Configuration["Authentication:GoogleOpenID:CallbackPath"];
                options.Authority = Configuration["Authentication:GoogleOpenID:Authority"];
                options.Scope.Add("email");
                options.Events = new OpenIdConnectEvents()
                {
                    OnTokenValidated = async context =>
                    {

                        if (context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "113435522445322472462")
                        {
                            var claim = new Claim(ClaimTypes.Role, "Admin");
                            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                            claimsIdentity.AddClaim(claim);
                        }
                        await Task.CompletedTask;
                    }
                };
            })
            .AddOpenIdConnect("OktaOpenID", options =>
            {
                // options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                
                options.ClientId = Configuration["Authentication:OktaOpenID:ClientId"];
                options.ClientSecret = Configuration["Authentication:OktaOpenID:ClientSecret"];
                options.CallbackPath = Configuration["Authentication:OktaOpenID:CallbackPath"];
                options.Authority = Configuration["Authentication:OktaOpenID:Authority"]+"/oauth2/default";
                
                options.RequireHttpsMetadata = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.SaveTokens=true;

                options.TokenValidationParameters = new TokenValidationParameters {
                    NameClaimType = "name",
                    RoleClaimType = "groups",
                    ValidateIssuer = true
                };                
            });
            /*.AddOktaMvc(new OktaMvcOptions
            {
                // Replace these values with your Okta configuration
                OktaDomain = "",
                ClientId = "",
                ClientSecret = "",
                AuthorizationServerId = "default",
                PostLogoutRedirectUri = "https://localhost:5001/auth/Login",
            });
            /*.AddGoogle(options =>
            {
                options.ClientId = Configuration["Authentication:GoogleOpenID:ClientId"];
                options.ClientSecret = Configuration["Authentication:GoogleOpenID:ClientSecret"];
                options.CallbackPath = Configuration["Authentication:GoogleOpenID:CallbackPath"];
                options.AuthorizationEndpoint += "?prompt=consent";
            });*/
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)//, AppIdentityDbContext dataContext
        {
            if (env.IsDevelopment())
            {
                //app.UseAuthentication();
                app.UseLiveReload();
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //to run migration at build
            
            //dataContext.Database.Migrate();

        //     using(var serviceScope = app.ApplicationServices.CreateScope)
        //     {
        //         var ct = serviceScope.ServiceProvider.GetService<AppIdentityDbContext>();
        //         ct.Database.Migrate();
        //     }
        }
    }
}
