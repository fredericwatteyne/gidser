// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MvcClient
{
    public static class Config
	{
		public static string Environment()
		{
			return System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
		}

		public static string Port()
		{
			return System.Environment.GetEnvironmentVariable("PORT");
		}

		public static string MvcClientUrl()
		{
			return System.Environment.GetEnvironmentVariable("MVC_CLIENT_URL");
		}

		public static string IdentityServerUrl()
		{
			return System.Environment.GetEnvironmentVariable("GIDSERIDENTITYSERVER_URL");
		}
    }
}