// -----------------------------------------------------------------------
// <copyright file="ServicesExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The <see cref="IServiceProvider"/> extensions.
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Uses services for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Action<IServiceCollection> configure)
        where T : CliConfiguration => UseServices(configuration, (_, builder) => configure(builder));

    /// <summary>
    /// Uses services for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T configuration,
        Action<ParseResult?, IServiceCollection> configure)
        where T : CliConfiguration
    {
        Invocation.BuilderAction.SetHandlers<ServiceCollection, IServiceProvider>(configuration.RootCommand, builder => builder.BuildServiceProvider(), configure);
        return configuration;
    }

    /// <summary>
    /// Gets the service provider from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this ParseResult parseResult) => Invocation.BuilderAction.GetInstance<IServiceProvider>(parseResult);

    /// <summary>
    /// Gets the service provider from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this CliCommand command) => Invocation.BuilderAction.GetInstance<IServiceProvider>(command);

    /// <summary>
    /// Gets the service provider from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this Invocation.CliAction action) => Invocation.BuilderAction.GetInstance<IServiceProvider>(action);
}