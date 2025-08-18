// -----------------------------------------------------------------------
// <copyright file="HostingExtensions.Services.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

/// <content>
/// The <see cref="HostingExtensions"/> for <see cref="IServiceProvider"/>.
/// </content>
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
public static partial class HostingExtensions
{
    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand)
        where T : RootCommand => UseServices(rootCommand, static _ => { });

    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : RootCommand => UseServices(
            rootCommand,
#if NET7_0_OR_GREATER
            static args => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args),
#else
            static () => new Microsoft.Extensions.Hosting.HostBuilder(),
#endif
            configure);

    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Action<ParseResult?, Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : RootCommand => UseServices(
            rootCommand,
#if NET7_0_OR_GREATER
            static args => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args),
#else
            static () => new Microsoft.Extensions.Hosting.HostBuilder(),
#endif
            configure);

    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory)
        where T : RootCommand => UseServices(rootCommand, hostBuilderFactory, static (_, _) => { });

    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory)
        where T : RootCommand => UseServices(rootCommand, hostBuilderFactory, static (_, _) => { });

    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : RootCommand => UseServices(rootCommand, hostBuilderFactory, (_, builder) => configure(builder));

    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : RootCommand => UseServices(rootCommand, hostBuilderFactory, (_, builder) => configure(builder));

    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Func<Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<ParseResult?, Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : RootCommand
    {
        Invocation.BuilderCommandLineAction.SetActions(
            rootCommand,
            _ => hostBuilderFactory(),
            static builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureServices((_, services) => configure(parseResult, services)));
        return rootCommand;
    }

    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Func<string[], Microsoft.Extensions.Hosting.IHostBuilder> hostBuilderFactory,
        Action<ParseResult?, Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
        where T : RootCommand
    {
        Invocation.BuilderCommandLineAction.SetActions(
            rootCommand,
            parseResult => hostBuilderFactory(parseResult?.UnmatchedTokens.ToArray() ?? []),
            static builder => builder.Build(),
            (parseResult, builder) => builder.ConfigureServices((_, services) => configure(parseResult, services)));
        return rootCommand;
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