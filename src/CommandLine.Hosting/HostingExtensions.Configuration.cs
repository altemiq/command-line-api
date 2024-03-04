// -----------------------------------------------------------------------
// <copyright file="HostingExtensions.Configuration.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

/// <content>
/// The <see cref="HostingExtensions"/> for <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
/// </content>
public static partial class HostingExtensions
{
    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration)
        where T : CliConfiguration => UseConfiguration(configuration, _ => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Action<Microsoft.Extensions.Configuration.IConfigurationBuilder> configure)
        where T : CliConfiguration => UseConfiguration(configuration, () => new Microsoft.Extensions.Hosting.HostBuilder(), configure);

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Action<ParseResult?, Microsoft.Extensions.Configuration.IConfigurationBuilder> configure)
        where T : CliConfiguration => UseConfiguration(configuration, () => new Microsoft.Extensions.Hosting.HostBuilder(), configure);

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory)
        where T : CliConfiguration => UseConfiguration(configuration, hostBuilderFactory, _ => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory)
        where T : CliConfiguration => UseConfiguration(configuration, hostBuilderFactory, _ => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<Microsoft.Extensions.Configuration.IConfigurationBuilder> configure)
        where T : CliConfiguration => UseConfiguration(configuration, hostBuilderFactory, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<Microsoft.Extensions.Configuration.IConfigurationBuilder> configure)
        where T : CliConfiguration => UseConfiguration(configuration, hostBuilderFactory, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<ParseResult?, Microsoft.Extensions.Configuration.IConfigurationBuilder> configure)
        where T : CliConfiguration
    {
        Invocation.BuilderAction.SetHandlers(
            configuration.RootCommand,
            _ => hostBuilderFactory(),
            builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureAppConfiguration((_, builder) => configure(parseResult, builder)));

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
    public static T UseConfiguration<T>(
        this T configuration,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<ParseResult?, Microsoft.Extensions.Configuration.IConfigurationBuilder> configure)
        where T : CliConfiguration
    {
        Invocation.BuilderAction.SetHandlers(
            configuration.RootCommand,
            parseResult => hostBuilderFactory(parseResult?.UnmatchedTokens.ToArray() ?? []),
            builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureAppConfiguration((_, builder) => configure(parseResult, builder)));

        return configuration;
    }

    /// <summary>
    /// Gets the configuration from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The configuration.</returns>
    public static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this ParseResult parseResult) => Invocation.InstanceAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(parseResult)?.GetConfiguration();

    /// <summary>
    /// Gets the configuration from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The configuration.</returns>
    public static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this CliCommand command) => Invocation.InstanceAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(command)?.GetConfiguration();

    /// <summary>
    /// Gets the configuration from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The configuration.</returns>
    public static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this Invocation.CliAction action) => Invocation.InstanceAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(action)?.GetConfiguration();

    private static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this Microsoft.Extensions.Hosting.IHost host) => (Microsoft.Extensions.Configuration.IConfiguration?)host.Services.GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration));
}