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
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
public static class ServicesExtensions
{
    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured configuration.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Action<IServiceCollection> configure)
        where T : RootCommand => UseServices(rootCommand, (_, builder) => configure(builder));

    /// <summary>
    /// Uses services for the root command.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="configure">The configure action.</param>
    /// <returns>The configured root command.</returns>
    public static T UseServices<T>(
        this T rootCommand,
        Action<ParseResult?, IServiceCollection> configure)
        where T : RootCommand
    {
        Invocation.BuilderCommandLineAction.SetActions<ServiceCollection, IServiceProvider>(rootCommand, static builder => builder.BuildServiceProvider(), configure);
        return rootCommand;
    }

    /// <summary>
    /// Gets the service provider from the parse result.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this ParseResult parseResult) => Invocation.InstanceCommandLineAction.GetInstance<IServiceProvider>(parseResult);

    /// <summary>
    /// Gets the service provider from the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this Command command) => Invocation.InstanceCommandLineAction.GetInstance<IServiceProvider>(command);

    /// <summary>
    /// Gets the service provider from the action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider? GetServices(this Invocation.CommandLineAction action) => Invocation.InstanceCommandLineAction.GetInstance<IServiceProvider>(action);
}