﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServer4.EntityFramework.DbContexts;
using System.Linq;
using IdentityServer4.EntityFramework.Mappers;

using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System;

namespace GidserIdentityServer
{
    public class Startup
	{
        public Startup(IHostingEnvironment env)
		{
			Console.WriteLine("Hello from IdentityServer");
			Console.WriteLine($"En: {env.EnvironmentName}");
            Console.WriteLine($"PORT: {Config.Port()}");
            Console.WriteLine($"GIDSERIDENTITYSERVER_URL: {Config.IdentityServerUrl()}");
			Console.WriteLine($"MVC_CLIENT_URL: {Config.MvcClientUrl()}");

            var url = $"http://*:{Config.Port()}/";
			Console.WriteLine($"Using Url: {url}");

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
			services.AddMvc();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            if (Config.Environment().Equals("Development"))
			{
				Console.WriteLine("Sqlite Database configuration");
				// configure identity server with in-memory users, but EF stores for clients and scopes
				services.AddIdentityServer()
					.AddTemporarySigningCredential()
					.AddTestUsers(Config.GetUsers())
					.AddConfigurationStore(builder =>
						builder.UseSqlite(Configuration["Connection"], options =>
							options.MigrationsAssembly(migrationsAssembly)))
					.AddOperationalStore(builder =>
						builder.UseSqlite(Configuration["Connection"], options =>
							options.MigrationsAssembly(migrationsAssembly)));
                
            } else if (Config.Environment().Equals("Staging"))
			{
				Console.WriteLine("Inmemory Database configuration");
				// configure identity server with in-memory users, but EF stores for clients and scopes
				services.AddIdentityServer()
					.AddInMemoryClients(Config.GetClients(Config.MvcClientUrl()))
					.AddInMemoryIdentityResources(Config.GetIdentityResources())
					.AddInMemoryApiResources(Config.GetApiResources())
					.AddTestUsers(Config.GetUsers())
					.AddTemporarySigningCredential();
            }

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // this will do the initial DB population
            //InitializeDatabase(app);

            loggerFactory.AddConsole(LogLevel.Debug);
            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                DisplayName = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com",
                ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo"
            });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients(Config.MvcClientUrl()))
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}