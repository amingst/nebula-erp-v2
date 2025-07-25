﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nebula.Services.Authentication.Shared.Helpers;
using Nebula.Services.Authentication.Shared.Middleware;
using Nebula.Services.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Shared.Extensions
{
    public static class JwtExtensions
    {
        public const string JWT_PRIVATE_KEY_ENVIRONMENT_NAME = "JWT_PRIV_KEY";
        public const string JWT_PUBLIC_KEY_ENVIRONMENT_NAME = "JWT_PUB_KEY";
        public const string JWT_COOKIE_NAME = "token";

        public static void AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<NebulaUserHelper>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = GetPublicKey(),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization();
        }

        public static void UseJwtAuthentication(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtCookieToBearerMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();
        }

        public static void UseJwtApiAuthentication(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtCookieToBearerMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
        }


        public static JsonWebKey GetPrivateKey()
        {
            return GetKeyFromEnvVar(JWT_PRIVATE_KEY_ENVIRONMENT_NAME);
        }

        public static JsonWebKey GetPublicKey()
        {
            return GetKeyFromEnvVar(JWT_PUBLIC_KEY_ENVIRONMENT_NAME);
        }

        private static JsonWebKey GetKeyFromEnvVar(string keyName)
        {
            var envKey = Environment.GetEnvironmentVariable(keyName, EnvironmentVariableTarget.Process);
            if (envKey == null)
                throw new Exception($"Environment variable not found {keyName}");

            return envKey.DecodeJsonWebKey();
        }
    }
}
