// -----------------------------------------------------------------------
// <copyright file="HelpExtensions.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// <see cref="CliConfiguration"/> extensions.
/// </summary>
internal static class HelpExtensions
{
    /// <summary>
    /// Uses the specified help.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure function.</param>
    /// <returns>The configuration for chaining.</returns>
    public static CliConfiguration ConfigureHelp(this CliConfiguration configuration, Action<Help.HelpBuilder> configure)
    {
        if (GetHelpAction(configuration.RootCommand) is { } helpAction)
        {
            configure(helpAction.Builder);
        }

        return configuration;

        static Help.HelpAction? GetHelpAction(CliCommand command)
        {
            return command.Options
                .OfType<Help.HelpOption>()
                .Select(option => GetHelpAction(option))
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
}