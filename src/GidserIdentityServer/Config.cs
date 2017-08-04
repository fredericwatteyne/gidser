﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace GidserIdentityServer
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

        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }


        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(string mvcClientUrl)
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    ClientName = "Resource Owner Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { $"{mvcClientUrl}/signin-oidc" },
                    PostLogoutRedirectUris = { $"{mvcClientUrl}/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowOfflineAccess = true
                },

                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { "http://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5003" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "karel",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Karel"),
                        new Claim("website", "https://karel.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "jos",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Jos"),
                        new Claim("website", "https://jos.com")
                    }
                }
            };
        }
    }
}