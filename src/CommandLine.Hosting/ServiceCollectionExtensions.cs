// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130, CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130, CheckNamespace

/// <summary>
/// The <see cref="IServiceCollection"/> extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the invocation lifetime.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configureOptions">The options to configure.</param>
    /// <returns>The services for chaining.</returns>
    public static IServiceCollection ConfigureInvocationLifetime(this IServiceCollection services, Action<System.CommandLine.Hosting.InvocationLifetimeOptions>? configureOptions = null)
    {
        if (configureOptions is { } configureOptionsAction)
        {
            _ = services.Configure(configureOptionsAction);
        }

        return services;
    }

    /// <summary>
    /// Adds the invocation lifetime to the services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The services for chaining.</returns>
    public static IServiceCollection AddInvocationLifetime(this IServiceCollection services)
    {
        return Contains(services, out var lifetime)
            ? Extensions.ServiceCollectionDescriptorExtensions.Replace(
                services,
                new ServiceDescriptor(
                    typeof(Hosting.IHostLifetime),
                    typeof(System.CommandLine.Hosting.InvocationLifetime),
                    lifetime))
            : services.AddScoped<Hosting.IHostLifetime, System.CommandLine.Hosting.InvocationLifetime>();

        // if this already contains an IHostLifetime
        static bool Contains(IServiceCollection serviceCollection, out ServiceLifetime lifetime)
        {
            if (serviceCollection.FirstOrDefault(static service => service.ServiceType == typeof(Hosting.IHostLifetime))
                is { } service)
            {
                lifetime = service.Lifetime;
                return true;
            }

            lifetime = default;
            return false;
        }
    }
}