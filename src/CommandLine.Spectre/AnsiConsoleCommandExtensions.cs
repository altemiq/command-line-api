// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleCommandExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

using Spectre.Console;

/// <summary>
/// The <see cref="AnsiConsole"/> <see cref="CliCommand"/> extensions.
/// </summary>
public static class AnsiConsoleCommandExtensions
{
    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <typeparam name="T">The type of command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <returns>The command for chaining.</returns>
    public static T AddFiglet<T>(this T command, string text, Color color)
        where T : CliCommand => AddFiglet(command, () => new FigletText(text).Color(color));

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <typeparam name="T">The type of command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="font">The figlet font.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <returns>The command for chaining.</returns>
    public static T AddFiglet<T>(this T command, FigletFont font, string text, Color color)
        where T : CliCommand => AddFiglet(command, () => new FigletText(font, text).Color(color));

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <typeparam name="T">The type of command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="text">The text.</param>
    /// <returns>The command for chaining.</returns>
    public static T AddFiglet<T>(this T command, FigletText text)
        where T : CliCommand => AddFiglet(command, () => text);

    private static T AddFiglet<T>(T command, Func<FigletText> getText)
        where T : CliCommand => command.ConfigureHelp(builder => builder.CustomizeLayout(_ => Help.HelpBuilder.Default.GetLayout().Prepend(helpContext =>
        {
            if (helpContext.Command == command)
            {
                AnsiConsole.Write(getText());
                return true;
            }

            return false;
        })));
}