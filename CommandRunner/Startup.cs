using System;
using System.Threading.Tasks;
using CommandRunner.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSDev.Tools.CommandRunner;

namespace CommandRunner
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddDistributedMemoryCache();

            services.AddSession(options => {
                options.CookieName = ".Session.CommandRunnerCookies";
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.CookieHttpOnly = true;
            });
            services.AddAuthorization(options => {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            services.AddScoped(typeof(JsonFileHelper));
            services.AddScoped(typeof(Runner));

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			UserHelper.GetUserAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            } else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseSession();

            app.UseCookieAuthentication(new CookieAuthenticationOptions() {
                AuthenticationScheme = "CommandRunnerCookies",
                LoginPath = new PathString("/Home/Login/"),
                AccessDeniedPath = new PathString("/Home/Forbidden/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                ExpireTimeSpan = TimeSpan.FromDays(1),
                Events = new CookieAuthenticationEvents {
                    OnValidatePrincipal = SessionHelper.ValidateAsync
                }

            });
            var webSocketOptions = new WebSocketOptions() {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);


            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Login}/{id?}");
            });

        }
    }
}
