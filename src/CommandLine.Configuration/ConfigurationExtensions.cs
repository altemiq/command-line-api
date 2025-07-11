﻿// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

using Microsoft.Extensions.Configuration;

/// <summary>
/// The <see cref="IConfiguration"/> extensions.
/// </summary>
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
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
        where T : CommandLineConfiguration => UseConfiguration(configuration, (_, builder) => configure(builder));

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
        where T : CommandLineConfiguration
    {
        Invocation.BuilderCommandLineAction.SetHandlers<ConfigurationBuilder, IConfiguration>(configuration.RootCommand, static builder => builder.Build(), configure);
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
        where T : CommandLineConfiguration => UseConfiguration(configuration, createBuilder, (_, builder) => configure(builder));

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
        where T : CommandLineConfiguration => UseConfiguration(configuration, createBuilder, (_, builder) => configure(builder));

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
        where T : CommandLineConfiguration => UseConfiguration(configuration, _ => createBuilder(), configure);

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
        where T : CommandLineConfiguration
    {
        Invocation.BuilderCommandLineAction.SetHandlers(configuration.RootCommand, createBuilder, static builder => builder.Build(), configure);
        return configuration;
    }

    /// <summary>
    /// Gets the configuration from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The configuration.</returns>
    public static IConfiguration? GetConfiguration(this ParseResult parseResult) => Invocation.InstanceCommandLineAction.GetInstance<IConfiguration>(parseResult);

    /// <summary>
    /// Gets the configuration from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The configuration.</returns>
    public static IConfiguration? GetConfiguration(this Command command) => Invocation.InstanceCommandLineAction.GetInstance<IConfiguration>(command);

    /// <summary>
    /// Gets the configuration from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The configuration.</returns>
    public static IConfiguration? GetConfiguration(this Invocation.CommandLineAction action) => Invocation.InstanceCommandLineAction.GetInstance<IConfiguration>(action);
}