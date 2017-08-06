// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
		{
			Console.WriteLine("Hello from Api");
			Console.WriteLine($"En: {env.EnvironmentName}");
			Console.WriteLine($"IdentityServerUrl: {Config.IdentityServerUrl()}");
			Console.WriteLine($"MvcClientUrl: {Config.MvcClientUrl()}");
			Console.WriteLine($"Port: {Config.Port()}");

			var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
			Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options=>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                policy.WithOrigins(Config.ApiUrl())
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // this uses the policy called "default"
            app.UseCors("default");

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = Config.IdentityServerUrl(),
                RequireHttpsMetadata = false,

                ApiName = "api1"
            });

            app.UseMvc();
        }
    }
}