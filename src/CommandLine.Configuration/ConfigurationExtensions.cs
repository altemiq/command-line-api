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
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
public static class ConfigurationExtensions
{
    /// <summary>
    /// Uses configuration for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseConfiguration<T>(
        this T rootCommand,
        Action<IConfigurationBuilder> configure)
        where T : RootCommand => UseConfiguration(rootCommand, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseConfiguration<T>(
        this T rootCommand,
        Action<ParseResult?, IConfigurationBuilder> configure)
        where T : RootCommand
    {
        Invocation.BuilderCommandLineAction.SetActions<ConfigurationBuilder, IConfiguration>(rootCommand, static builder => builder.Build(), configure);
        return rootCommand;
    }

    /// <summary>
    /// Uses configuration for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="createBuilder">The builder creator.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseConfiguration<T>(
        this T rootCommand,
        Func<IConfigurationBuilder> createBuilder,
        Action<IConfigurationBuilder> configure)
        where T : RootCommand => UseConfiguration(rootCommand, createBuilder, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="createBuilder">The builder creator.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseConfiguration<T>(
        this T rootCommand,
        Func<ParseResult?, IConfigurationBuilder> createBuilder,
        Action<IConfigurationBuilder> configure)
        where T : RootCommand => UseConfiguration(rootCommand, createBuilder, (_, builder) => configure(builder));

    /// <summary>
    /// Uses configuration for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="createBuilder">The builder creator.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseConfiguration<T>(
        this T rootCommand,
        Func<IConfigurationBuilder> createBuilder,
        Action<ParseResult?, IConfigurationBuilder> configure)
        where T : RootCommand => UseConfiguration(rootCommand, _ => createBuilder(), configure);

    /// <summary>
    /// Uses configuration for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="createBuilder">The builder creator.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseConfiguration<T>(
        this T rootCommand,
        Func<ParseResult?, IConfigurationBuilder> createBuilder,
        Action<ParseResult?, IConfigurationBuilder> configure)
        where T : RootCommand
    {
        Invocation.BuilderCommandLineAction.SetActions(rootCommand, createBuilder, static builder => builder.Build(), configure);
        return rootCommand;
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