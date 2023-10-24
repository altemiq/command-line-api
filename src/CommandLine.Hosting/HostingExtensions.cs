// -----------------------------------------------------------------------
// <copyright file="HostingExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

/// <summary>
/// See <see cref="Microsoft.Extensions.Hosting"/> extensions.
/// </summary>
public static class HostingExtensions
{
    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseConfiguration(
        this CliConfiguration configuration) => UseConfiguration(configuration, _ => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseConfiguration(
        this CliConfiguration configuration,
        Action<Microsoft.Extensions.Configuration.IConfigurationBuilder> configure) => UseConfiguration(configuration, () => new Microsoft.Extensions.Hosting.HostBuilder(), configure);

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseConfiguration(
        this CliConfiguration configuration,
        Action<ParseResult, Microsoft.Extensions.Configuration.IConfigurationBuilder> configure) => UseConfiguration(configuration, () => new Microsoft.Extensions.Hosting.HostBuilder(), configure);

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseConfiguration(
        this CliConfiguration configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory) => UseConfiguration(configuration, hostBuilderFactory, _ => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseConfiguration(
        this CliConfiguration configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<Microsoft.Extensions.Configuration.IConfigurationBuilder> configure) => UseConfiguration(configuration, hostBuilderFactory, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseConfiguration(
        this CliConfiguration configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<ParseResult, Microsoft.Extensions.Configuration.IConfigurationBuilder> configure)
    {
        Invocation.BuilderAction.SetHandlers(
            configuration.RootCommand,
            hostBuilderFactory,
            builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureAppConfiguration((_, builder) => configure(parseResult, builder)));

        return configuration;
    }

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseServices(
        this CliConfiguration configuration) => UseConfiguration(configuration, _ => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseServices(
        this CliConfiguration configuration,
        Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> configure) => UseServices(configuration, () => new Microsoft.Extensions.Hosting.HostBuilder(), configure);

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseServices(
        this CliConfiguration configuration,
        Action<ParseResult, Microsoft.Extensions.DependencyInjection.IServiceCollection> configure) => UseServices(configuration, () => new Microsoft.Extensions.Hosting.HostBuilder(), configure);

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseServices(
        this CliConfiguration configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory) => UseServices(configuration, hostBuilderFactory, (_, _) => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseServices(
        this CliConfiguration configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> configure) => UseServices(configuration, hostBuilderFactory, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseServices(
        this CliConfiguration configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<ParseResult, Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
    {
        Invocation.BuilderAction.SetHandlers(
            configuration.RootCommand,
            hostBuilderFactory,
            builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureServices((_, services) => configure(parseResult, services)));
        return configuration;
    }

    /// <summary>
    /// Gets the configuration from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The configuration.</returns>
    public static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this ParseResult parseResult) => Invocation.BuilderAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(parseResult)?.GetConfiguration();

    /// <summary>
    /// Gets the configuration from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The configuration.</returns>
    public static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this CliCommand command) => Invocation.BuilderAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(command)?.GetConfiguration();

    /// <summary>
    /// Gets the configuration from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The configuration.</returns>
    public static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this Invocation.CliAction action) => Invocation.BuilderAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(action)?.GetConfiguration();

    /// <summary>
    /// Gets the service provider from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this ParseResult parseResult) => Invocation.BuilderAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(parseResult)?.GetServices();

    /// <summary>
    /// Gets the service provider from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this CliCommand command) => Invocation.BuilderAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(command)?.GetServices();

    /// <summary>
    /// Gets the service provider from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this Invocation.CliAction action) => Invocation.BuilderAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(action)?.GetServices();

    private static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this Microsoft.Extensions.Hosting.IHost host) => (Microsoft.Extensions.Configuration.IConfiguration)host.Services.GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration));

    private static IServiceProvider GetServices(this Microsoft.Extensions.Hosting.IHost host) => host.Services;
}