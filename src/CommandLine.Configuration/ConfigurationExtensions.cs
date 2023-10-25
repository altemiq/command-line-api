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
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Action<IConfigurationBuilder> configure)
        where T : CliConfiguration => UseConfiguration(configuration, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Action<ParseResult?, IConfigurationBuilder> configure)
        where T : CliConfiguration
    {
        Invocation.BuilderAction.SetHandlers<ConfigurationBuilder, IConfiguration>(configuration.RootCommand, builder => builder.Build(), configure);
        return configuration;
    }

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="createBuilder">The builder creator.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Func<IConfigurationBuilder> createBuilder,
        Action<IConfigurationBuilder> configure)
        where T : CliConfiguration => UseConfiguration(configuration, createBuilder, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="createBuilder">The builder creator.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Func<ParseResult?, IConfigurationBuilder> createBuilder,
        Action<IConfigurationBuilder> configure)
        where T : CliConfiguration => UseConfiguration(configuration, createBuilder, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="createBuilder">The builder creator.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Func<IConfigurationBuilder> createBuilder,
        Action<ParseResult?, IConfigurationBuilder> configure)
        where T : CliConfiguration => UseConfiguration(configuration, _ => createBuilder(), configure);

    /// <summary>
    /// Uses configuration for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="createBuilder">The builder creator.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseConfiguration<T>(
        this T configuration,
        Func<ParseResult?, IConfigurationBuilder> createBuilder,
        Action<ParseResult?, IConfigurationBuilder> configure)
        where T : CliConfiguration
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