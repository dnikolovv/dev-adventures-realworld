using Conduit.Api.OperationFilters;
using Conduit.Core.Configuration;
using Conduit.Data.Entities;
using Conduit.Data.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conduit.Api.Configuration
{
    public static class DependenciesConfiguration
    {
        public static void AddDbContext(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(connectionString));
        }

        public static void AddJwtIdentity(this IServiceCollection services, IConfigurationSection jwtConfiguration)
        {
            services.AddIdentity<User, IdentityRole>(opts =>
                {
                    opts.User.RequireUniqueEmail = true;
                    opts.Password.RequireDigit = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var signingKey = new SymmetricSecurityKey(
                Encoding.Default.GetBytes(jwtConfiguration["Secret"]));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfiguration[nameof(JwtConfiguration.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtConfiguration[nameof(JwtConfiguration.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.Configure<JwtConfiguration>(options =>
            {
                options.Issuer = jwtConfiguration[nameof(JwtConfiguration.Issuer)];
                options.Audience = jwtConfiguration[nameof(JwtConfiguration.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtConfiguration[nameof(JwtConfiguration.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
                configureOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = (context) =>
                    {
                        // This is to enable parsing the token from an authorization header in the format 'Token {token}'
                        var token = context.HttpContext.Request.Headers["Authorization"];
                        if (token.Count > 0 && token[0].StartsWith("Token ", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = token[0].Substring("Token ".Length).Trim();
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });
        }

        public static IMvcBuilder AddJsonOptions(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddJsonOptions(options =>
            {
                var converter = new IsoDateTimeConverter
                {
                    DateTimeStyles = System.Globalization.DateTimeStyles.AdjustToUniversal,

                    // We need to be THAT specific in order to pass the Postman tests which test our
                    // returned date against javascript's toISOString().
                    DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'"
                };

                options.SerializerSettings.Converters.Add(converter);
                options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            });

            return mvcBuilder;
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new Info { Title = "Conduit.Api", Version = "v1" });
                setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Conduit.Api.Documentation.xml"));

                setup.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Enter 'Bearer {token}' (don't forget to add 'bearer') into the field below.", Name = "Authorization", Type = "apiKey" });

                setup.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Enumerable.Empty<string>() },
                });

                setup.OperationFilter<OptionOperationFilter>();
            });
        }
    }
}
