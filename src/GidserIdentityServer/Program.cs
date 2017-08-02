// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace GidserIdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
			Console.Title = "IdentityServer";

			var url = $"http://*:{Environment.GetEnvironmentVariable("PORT")}/";

			Console.WriteLine($"Using Url: {url}");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(url)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}