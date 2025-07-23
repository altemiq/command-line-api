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
    public static IServiceCollection ConfigureInvocationLifetime(this IServiceCollection services, Action<System.CommandLine.Hosting.InvocationLifetimeOptions>? configureOptions = null) =>
        configureOptions is { } configureOptionsAction ? services.Configure(configureOptionsAction) : services;

    /// <summary>
    /// Adds the invocation lifetime to the services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The services for chaining.</returns>
    public static IServiceCollection AddInvocationLifetime(this IServiceCollection services) =>
        services.FirstOrDefault(static service => service.ServiceType == typeof(Hosting.IHostLifetime)) is { } service // if this already contains an IHostLifetime
            ? Extensions.ServiceCollectionDescriptorExtensions.Replace(
                services,
                new ServiceDescriptor(
                    typeof(Hosting.IHostLifetime),
                    typeof(System.CommandLine.Hosting.InvocationLifetime),
                    service.Lifetime))
            : services.AddScoped<Hosting.IHostLifetime, System.CommandLine.Hosting.InvocationLifetime>();
}