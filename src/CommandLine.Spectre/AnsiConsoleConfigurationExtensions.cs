// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleConfigurationExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// Extensions using <see cref="Spectre.Console"/>.
/// </summary>
public static class AnsiConsoleConfigurationExtensions
{
    /// <summary>
    /// Adds the specified figlet to the <see cref="CliRootCommand"/> help.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <param name="console">The console.</param>
    /// <returns>The configuration for chaining.</returns>
    public static T AddFiglet<T>(this T configuration, string text, Color color, IAnsiConsole? console = default)
        where T : CliConfiguration
    {
        _ = configuration.RootCommand.AddFiglet(text, color, console);
        return configuration;
    }

    /// <summary>
    /// Adds the specified figlet to the <see cref="CliRootCommand"/> help.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="font">The figlet font.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <param name="console">The console.</param>
    /// <returns>The configuration for chaining.</returns>
    public static T AddFiglet<T>(this T configuration, FigletFont font, string text, Color color, IAnsiConsole? console = default)
        where T : CliConfiguration
    {
        _ = configuration.RootCommand.AddFiglet(font, text, color, console);
        return configuration;
    }

    /// <summary>
    /// Adds the specified figlet to the <see cref="CliRootCommand"/> help.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="text">The text.</param>
    /// <param name="console">The console.</param>
    /// <returns>The configuration for chaining.</returns>
    public static T AddFiglet<T>(this T configuration, FigletText text, IAnsiConsole? console = default)
        where T : CliConfiguration
    {
        _ = configuration.RootCommand.AddFiglet(text, console);
        return configuration;
    }
}