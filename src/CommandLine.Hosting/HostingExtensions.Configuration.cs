// -----------------------------------------------------------------------
// <copyright file="HostingExtensions.Configuration.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

/// <content>
/// The <see cref="HostingExtensions"/> for <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
/// </content>
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
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
        where T : CommandLineConfiguration => UseConfiguration(configuration, static _ => { });

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
        where T : CommandLineConfiguration => UseConfiguration(
            configuration,
#if NET7_0_OR_GREATER
            static args => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args),
#else
            static () => new Microsoft.Extensions.Hosting.HostBuilder(),
#endif
            configure);

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
        where T : CommandLineConfiguration => UseConfiguration(
            configuration,
#if NET7_0_OR_GREATER
            static args => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args),
#else
            static () => new Microsoft.Extensions.Hosting.HostBuilder(),
#endif
            configure);

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
        where T : CommandLineConfiguration => UseConfiguration(configuration, hostBuilderFactory, static _ => { });

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
        where T : CommandLineConfiguration => UseConfiguration(configuration, hostBuilderFactory, static _ => { });

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
        where T : CommandLineConfiguration => UseConfiguration(configuration, hostBuilderFactory, (_, builder) => configure(builder));

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
        where T : CommandLineConfiguration => UseConfiguration(configuration, hostBuilderFactory, (_, builder) => configure(builder));

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
        where T : CommandLineConfiguration
    {
        Invocation.BuilderCommandLineAction.SetActions(
            configuration.RootCommand,
            _ => hostBuilderFactory(),
            static builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureAppConfiguration((_, b) => configure(parseResult, b)));

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
        where T : CommandLineConfiguration
    {
        Invocation.BuilderCommandLineAction.SetActions(
            configuration.RootCommand,
            parseResult => hostBuilderFactory(parseResult?.UnmatchedTokens.ToArray() ?? []),
            static builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureAppConfiguration((_, b) => configure(parseResult, b)));

        return configuration;
    }

    /// <summary>
    /// Gets the configuration from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The configuration.</returns>
    public static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this ParseResult parseResult) => Invocation.InstanceCommandLineAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(parseResult)?.GetConfiguration();

    /// <summary>
    /// Gets the configuration from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The configuration.</returns>
    public static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this Command command) => Invocation.InstanceCommandLineAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(command)?.GetConfiguration();

    /// <summary>
    /// Gets the configuration from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The configuration.</returns>
    public static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this Invocation.CommandLineAction action) => Invocation.InstanceCommandLineAction.GetInstance<Microsoft.Extensions.Hosting.IHost>(action)?.GetConfiguration();

    private static Microsoft.Extensions.Configuration.IConfiguration? GetConfiguration(this Microsoft.Extensions.Hosting.IHost host) => (Microsoft.Extensions.Configuration.IConfiguration?)host.Services.GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration));
}