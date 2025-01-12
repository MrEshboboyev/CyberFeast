using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BuildingBlocks.Core.Exception.Types;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Security.Jwt;

/// <summary>
/// Provides extension methods for registering custom JWT authentication and authorization services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds custom JWT authentication to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration for JWT options.</param>
    /// <param name="configurator">Optional action to configure JWT options.</param>
    /// <returns>The updated authentication builder.</returns>
    public static AuthenticationBuilder AddCustomJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<JwtOptions>? configurator = null
    )
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        var jwtOptions = configuration.BindOptions<JwtOptions>();
        configurator?.Invoke(jwtOptions);

        // Add JWT options to the dependency injection.
        services.AddValidationOptions<JwtOptions>(opt => configurator?.Invoke(opt));

        services.TryAddTransient<IJwtService, JwtService>();

        return services
            .AddAuthentication() // No default scheme specified.
            .AddJwtBearer(options =>
            {
                options.Audience = jwtOptions.Audience;
                options.SaveToken = true;
                options.RefreshOnIssuerKeyNotFound = false;
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    SaveSigninToken = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            throw new UnAuthorizedException("The Token is expired.");
                        }

                        throw new IdentityException(
                            context.Exception.Message,
                            statusCode: StatusCodes.Status500InternalServerError
                        );
                    },
                    OnChallenge = _ => Task.CompletedTask,
                    OnForbidden = _ => throw new ForbiddenException("You are not authorized to access this resource.")
                };
            });
    }

    /// <summary>
    /// Adds custom authorization policies to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="claimPolicies">Optional list of claim policies.</param>
    /// <param name="rolePolicies">Optional list of role policies.</param>
    /// <param name="scheme">The authentication scheme to use.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCustomAuthorization(
        this IServiceCollection services,
        IList<ClaimPolicy>? claimPolicies = null,
        IList<RolePolicy>? rolePolicies = null,
        string scheme = JwtBearerDefaults.AuthenticationScheme
    )
    {
        services.AddAuthorization(authorizationOptions =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(scheme);
            defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            authorizationOptions.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

            if (claimPolicies is not null)
            {
                foreach (var policy in claimPolicies)
                {
                    authorizationOptions.AddPolicy(
                        policy.Name,
                        x =>
                        {
                            x.AuthenticationSchemes.Add(scheme);
                            foreach (var policyClaim in policy.Claims)
                            {
                                x.RequireClaim(policyClaim.Type, policyClaim.Value);
                            }
                        }
                    );
                }
            }

            if (rolePolicies == null) return;

            foreach (var rolePolicy in rolePolicies)
            {
                authorizationOptions.AddPolicy(
                    rolePolicy.Name,
                    policyBuilder =>
                    {
                        policyBuilder.AuthenticationSchemes.Add(scheme);
                        policyBuilder.RequireRole(rolePolicy.Roles);
                    }
                );
            }
        });

        return services;
    }

    /// <summary>
    /// Adds external login providers to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration for external login providers.</param>
    public static void AddExternalLogins(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.BindOptions<JwtOptions>(nameof(JwtOptions));
        jwtOptions.NotBeNull();

        if (jwtOptions.GoogleLoginConfigs != null)
        {
            services
                .AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = jwtOptions.GoogleLoginConfigs.ClientId;
                    googleOptions.ClientSecret = jwtOptions.GoogleLoginConfigs.ClientSecret;
                    googleOptions.SaveTokens = true;
                });
        }
    }
}