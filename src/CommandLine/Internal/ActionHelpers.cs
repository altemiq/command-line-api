// -----------------------------------------------------------------------
// <copyright file="ActionHelpers.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Internal;

/// <summary>
/// The <see cref="Invocation.CliAction"/> helpers.
/// </summary>
internal static class ActionHelpers
{
    /// <summary>
    /// Gets the help action for the specified command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The help action.</returns>
    public static Help.HelpAction? GetHelpAction(CliCommand command)
    {
        return command.Options
            .OfType<Help.HelpOption>()
            .Select(GetHelpAction)
            .FirstOrDefault(action => action is not null);

        static Help.HelpAction? GetHelpAction(CliOption option)
        {
            return GetHelpActionCore(option.Action);

            static Help.HelpAction? GetHelpActionCore(Invocation.CliAction? action)
            {
                return action switch
                {
                    Help.HelpAction helpAction => helpAction,
                    Invocation.INestedAction nestedAction => GetHelpActionCore(nestedAction.Action),
                    _ => default,
                };
            }
        }
    }
}