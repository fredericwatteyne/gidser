using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace MvcClient
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
		{
			Console.WriteLine("Hello from Mvc Client");
			Console.WriteLine($"En: {env.EnvironmentName}");
			Console.WriteLine($"PORT: {Config.Port()}");
			Console.WriteLine($"IdentityServerUrl: {Config.IdentityServerUrl()}");
			Console.WriteLine($"MvcClientUrl: {Config.MvcClientUrl()}");
			Console.WriteLine($"ApiUrl: {Config.ApiUrl()}");

			var url = $"http://*:{Config.Port()}/";
			Console.WriteLine($"Using Url: {url}");
			
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",

                Authority = Config.IdentityServerUrl(),
                RequireHttpsMetadata = false,

                ClientId = "mvc",
                ClientSecret = "secret",

                ResponseType = "code id_token",
                Scope = { "api1", "offline_access" },

                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true
            });
            
            app.UseStaticFiles();


		    //app.UseIdentity();

		    // Adds IdentityServer
		    //app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
}