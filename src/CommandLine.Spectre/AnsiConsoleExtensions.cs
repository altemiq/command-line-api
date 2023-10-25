// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

using Spectre.Console;

/// <summary>
/// Extensions using <see cref="Spectre.Console"/>.
/// </summary>
public static class AnsiConsoleExtensions
{
    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <returns>The configuration for chaining.</returns>
    public static CliConfiguration AddFiglet(this CliConfiguration configuration, string text, Color color) => AddFiglet(configuration, () => new FigletText(text).Color(color));

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <returns>The command for chaining.</returns>
    public static CliCommand AddFiglet(this CliCommand command, string text, Color color) => AddFiglet(command, () => new FigletText(text).Color(color));

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="font">The figlet font.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <returns>The configuration for chaining.</returns>
    public static CliConfiguration AddFiglet(this CliConfiguration configuration, FigletFont font, string text, Color color) => AddFiglet(configuration, () => new FigletText(font, text).Color(color));

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="font">The figlet font.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <returns>The command for chaining.</returns>
    public static CliCommand AddFiglet(this CliCommand command, FigletFont font, string text, Color color) => AddFiglet(command, () => new FigletText(font, text).Color(color));

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="text">The text.</param>
    /// <returns>The configuration for chaining.</returns>
    public static CliConfiguration AddFiglet(this CliConfiguration configuration, FigletText text) => AddFiglet(configuration, () => text);

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="text">The text.</param>
    /// <returns>The command for chaining.</returns>
    public static CliCommand AddFiglet(this CliCommand command, FigletText text) => AddFiglet(command, () => text);

    private static CliConfiguration AddFiglet(CliConfiguration configuration, Func<FigletText> getText)
    {
        AddFiglet(configuration.RootCommand, getText);
        return configuration;
    }

    private static CliCommand AddFiglet(CliCommand command, Func<FigletText> getText) => command.ConfigureHelp(builder => builder.CustomizeLayout(_ => Help.HelpBuilder.Default.GetLayout().Prepend(helpContext =>
    {
        if (helpContext.Command == command)
        {
            AnsiConsole.Write(getText());
            return true;
        }

        return false;
    })));
}