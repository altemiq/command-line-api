// -----------------------------------------------------------------------
// <copyright file="HelpCommandExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The <see cref="Help"/> <see cref="Command"/> extensions.
/// </summary>
public static class HelpCommandExtensions
{
    /// <summary>
    /// Configures the help.
    /// </summary>
    /// <typeparam name="T">The type of command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="configure">The configure function.</param>
    /// <returns>The command for chaining.</returns>
    public static T ConfigureHelp<T>(this T command, Action<Help.HelpBuilder> configure)
        where T : Command
    {
        if (Internal.CommandLineActionHelpers.GetHelpAction((Symbol)command) is { } helpAction)
        {
            configure(helpAction.Builder);
        }

        return command;
    }
}