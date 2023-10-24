// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

using Microsoft.Extensions.Configuration;

/// <summary>
/// The <see cref="IConfiguration"/> extensions.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseConfiguration(this CliConfiguration configuration) => UseConfiguration(configuration, _ => { });

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseConfiguration(
        this CliConfiguration configuration,
        Action<IConfigurationBuilder> configure)
    {
        Invocation.BuilderAction.SetHandlers<ConfigurationBuilder, IConfiguration>(configuration.RootCommand, builder => builder.Build(), configure);
        return configuration;
    }

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="createBuilder">The builder creator.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static CliConfiguration UseConfiguration(
        this CliConfiguration configuration,
        Func<IConfigurationBuilder> createBuilder,
        Action<IConfigurationBuilder> configure)
    {
        Invocation.BuilderAction.SetHandlers(configuration.RootCommand, createBuilder, builder => builder.Build(), configure);
        return configuration;
    }

    /// <summary>
    /// Gets the configuration from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The configuration.</returns>
    public static IConfiguration? GetConfiguration(this ParseResult parseResult) => Invocation.BuilderAction.GetInstance<IConfiguration>(parseResult);

    /// <summary>
    /// Gets the configuration from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The configuration.</returns>
    public static IConfiguration? GetConfiguration(this CliCommand command) => Invocation.BuilderAction.GetInstance<IConfiguration>(command);

    /// <summary>
    /// Gets the configuration from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The configuration.</returns>
    public static IConfiguration? GetConfiguration(this Invocation.CliAction action) => Invocation.BuilderAction.GetInstance<IConfiguration>(action);
}