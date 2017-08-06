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
using IdentityModel;

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
			Console.WriteLine($"DATABASE_URL: {Environment.GetEnvironmentVariable("DATABASE_URL")}");

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


            if (Config.Environment().Equals("Development"))
			{
                DbConnectionSetting = DbConnection.Sqlite;
            }
            else if (Config.Environment().Equals("Staging"))
            {
                DbConnectionSetting = DbConnection.Postgres;
            }

            SetupDb(services);
        }

        private enum DbConnection 
        {
            InMemory,
            Sqlite,
            Postgres
        }

        private DbConnection DbConnectionSetting;

        public void SetupDb(IServiceCollection services) 
        {
			var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            switch (DbConnectionSetting) 
            {
				case DbConnection.InMemory:
					Console.WriteLine("InMemory Database configuration");
	                // configure identity server with in-memory users, but EF stores for clients and scopes
	                services.AddIdentityServer()
	                    .AddInMemoryClients(Config.GetClients(Config.MvcClientUrl()))
	                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
	                    .AddInMemoryApiResources(Config.GetApiResources())
	                    .AddTestUsers(Config.GetUsers())
	                    .AddTemporarySigningCredential();
                    break;


                case DbConnection.Sqlite:

					Console.WriteLine("Sqlite Database configuration");
	                // configure identity server with in-memory users, but EF stores for clients and scopes
	                services.AddIdentityServer()
	                    .AddTemporarySigningCredential()
	                    .AddTestUsers(Config.GetUsers())
	                    .AddConfigurationStore(builder =>
	                        builder.UseSqlite("Filename=MyDatabase.db", options =>
	                            options.MigrationsAssembly(migrationsAssembly)))
	                    .AddOperationalStore(builder =>
	                        builder.UseSqlite("Filename=MyDatabase.db", options =>
	                            options.MigrationsAssembly(migrationsAssembly)));
                    break;

                case DbConnection.Postgres:

                    Console.WriteLine("Postgres Database configuration");
					services.AddIdentityServer()
                        .AddDeveloperSigningCredential()
	                    .AddConfigurationStore(builder =>
	                        builder.UseNpgsql(Config.PostgresDBConnectionString(), options =>
	                                options.MigrationsAssembly(migrationsAssembly)))
	                    .AddOperationalStore(builder =>
	                        builder.UseNpgsql(Config.PostgresDBConnectionString(), options =>
	                                options.MigrationsAssembly(migrationsAssembly)));
                    break;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            InitializeDatabase(app);

            loggerFactory.AddConsole(LogLevel.Debug);
            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
				DisplayName = "Google",
				SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                ClientId = "370788744957-c4ntci0lu3pmu2jomobosm2upn0n5ajt.apps.googleusercontent.com",
                ClientSecret = "m5hPRUDJB5iXEM9mDbwqPb4N"
            });

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        private void InitializeDatabase(IApplicationBuilder app)
		{
            if (DbConnectionSetting == DbConnection.InMemory)
				return;
            
			Console.WriteLine("InitializeDatabase");

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var grantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
			    grantContext.Database.Migrate();

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