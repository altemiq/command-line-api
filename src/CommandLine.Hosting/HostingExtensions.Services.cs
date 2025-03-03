// -----------------------------------------------------------------------
// <copyright file="HostingExtensions.Services.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

/// <content>
/// The <see cref="HostingExtensions"/> for <see cref="IServiceProvider"/>.
/// </content>
public static partial class HostingExtensions
{
    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration)
        where T : CommandLineConfiguration => UseServices(configuration, static _ => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : CommandLineConfiguration => UseServices(configuration, static () => new Microsoft.Extensions.Hosting.HostBuilder(), configure);

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Action<ParseResult?, Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : CommandLineConfiguration => UseServices(configuration, static () => new Microsoft.Extensions.Hosting.HostBuilder(), configure);

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory)
        where T : CommandLineConfiguration => UseServices(configuration, hostBuilderFactory, static (_, _) => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory)
        where T : CommandLineConfiguration => UseServices(configuration, hostBuilderFactory, static (_, _) => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : CommandLineConfiguration => UseServices(configuration, hostBuilderFactory, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : CommandLineConfiguration => UseServices(configuration, hostBuilderFactory, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<ParseResult?, Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : CommandLineConfiguration
    {
        Invocation.BuilderCommandLineAction.SetHandlers(
            configuration.RootCommand,
            _ => hostBuilderFactory(),
            static builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureServices((_, services) => configure(parseResult, services)));
        return configuration;
    }

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<ParseResult?, Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : CommandLineConfiguration
    {
        Invocation.BuilderCommandLineAction.SetHandlers(
            configuration.RootCommand,
            parseResult => hostBuilderFactory(parseResult?.UnmatchedTokens.ToArray() ?? []),
            static builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureServices((_, services) => configure(parseResult, services)));
        return configuration;
    }

    /// <summary>
    /// Gets the service provider from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this ParseResult parseResult) => Invocation.InstanceCommandLineAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(parseResult)?.GetServices();

    /// <summary>
    /// Gets the service provider from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this Command command) => Invocation.InstanceCommandLineAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(command)?.GetServices();

    /// <summary>
    /// Gets the service provider from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this Invocation.CommandLineAction action) => Invocation.InstanceCommandLineAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(action)?.GetServices();

    private static IServiceProvider GetServices(this Microsoft.Extensions.Hosting.IHost host) => host.Services;
}