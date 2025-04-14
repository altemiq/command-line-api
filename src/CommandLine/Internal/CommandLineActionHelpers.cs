// -----------------------------------------------------------------------
// <copyright file="CommandLineActionHelpers.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Internal;

/// <summary>
/// The <see cref="Invocation.CommandLineAction"/> helpers.
/// </summary>
internal static class CommandLineActionHelpers
{
    /// <summary>
    /// Gets the help action for the specified symbol.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The help action.</returns>
    public static Help.HelpAction? GetHelpAction(Symbol symbol) => CommandHelpers.GetRootCommand(symbol) is { } rootCommand
        ? GetHelpAction(rootCommand)
        : default;

    /// <summary>
    /// Gets the help action for the specified command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The help action.</returns>
    public static Help.HelpAction? GetHelpAction(Command command)
    {
        return command.Options
            .OfType<Help.HelpOption>()
            .Select(HelpActionFromOption)
            .FirstOrDefault(static action => action is not null);

        static Help.HelpAction? HelpActionFromOption(Option option)
        {
            return GetHelpActionCore(option.Action);

            static Help.HelpAction? GetHelpActionCore(Invocation.CommandLineAction? action)
            {
                return action switch
                {
                    Help.HelpAction helpAction => helpAction,
                    Invocation.INestedCommandLineAction nestedAction => GetHelpActionCore(nestedAction.Action),
                    _ => default,
                };
            }
        }
    }
}