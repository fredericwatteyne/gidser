// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace MvcClient
{
    public static class Config
	{
		public static string Port()
		{
			return Environment.GetEnvironmentVariable("PORT");
		}

		public static string MvcClientUrl()
		{
			return Environment.GetEnvironmentVariable("MVC_CLIENT_URL");
		}

		public static string IdentityServerUrl()
		{
			return Environment.GetEnvironmentVariable("GIDSERIDENTITYSERVER_URL");
		}
    }
}