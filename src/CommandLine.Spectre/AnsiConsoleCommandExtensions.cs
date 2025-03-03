// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleCommandExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The <see cref="global::Spectre.Console.AnsiConsole"/> <see cref="Command"/> extensions.
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
    /// <param name="console">The console.</param>
    /// <returns>The command for chaining.</returns>
    public static T AddFiglet<T>(this T command, string text, ConsoleColor color, IAnsiConsole? console = default)
        where T : Command => AddFiglet(command, text, Color.FromConsoleColor(color), console);

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <typeparam name="T">The type of command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <param name="console">The console.</param>
    /// <returns>The command for chaining.</returns>
    public static T AddFiglet<T>(this T command, string text, Drawing.Color color, IAnsiConsole? console = default)
        where T : Command => AddFiglet(command, text, new Color(color.R, color.G, color.B), console);

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <typeparam name="T">The type of command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <param name="console">The console.</param>
    /// <returns>The command for chaining.</returns>
    public static T AddFiglet<T>(this T command, string text, Color color, IAnsiConsole? console = default)
        where T : Command => AddFiglet(command, () => new FigletText(text).Color(color), console);

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <typeparam name="T">The type of command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="font">The figlet font.</param>
    /// <param name="text">The text.</param>
    /// <param name="color">The color.</param>
    /// <param name="console">The console.</param>
    /// <returns>The command for chaining.</returns>
    public static T AddFiglet<T>(this T command, FigletFont font, string text, Color color, IAnsiConsole? console = default)
        where T : Command => AddFiglet(command, () => new FigletText(font, text).Color(color), console);

    /// <summary>
    /// Adds the specified figlet to the command help.
    /// </summary>
    /// <typeparam name="T">The type of command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="text">The text.</param>
    /// <param name="console">The console.</param>
    /// <returns>The command for chaining.</returns>
    public static T AddFiglet<T>(this T command, FigletText text, IAnsiConsole? console = default)
        where T : Command => AddFiglet(command, () => text, console);

    private static T AddFiglet<T>(T command, Func<FigletText> getText, IAnsiConsole? console = default)
        where T : Command
    {
        if (Internal.CommandLineActionHelpers.GetHelpAction((Symbol)command) is { } helpAction)
        {
            helpAction.Builder.CustomizeLayout(_ => Help.HelpBuilder.Default.GetLayout().Prepend(helpContext =>
            {
                return AddFigletCore(command, getText, console.GetValueOrDefault());

                bool AddFigletCore(T command, Func<FigletText> getText, IAnsiConsole console)
                {
                    if (helpContext.Command == command)
                    {
                        console.Write(getText());
                        return true;
                    }

                    return false;
                }
            }));

            return command;
        }

        throw new InvalidOperationException(Spectre.Properties.Resources.HelpCommandNotFound);
    }
}