// -----------------------------------------------------------------------
// <copyright file="HelpExtensions.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// <see cref="CliConfiguration"/> extensions.
/// </summary>
public static class HelpExtensions
{
    /// <summary>
    /// Customizes the help for the symbol.
    /// </summary>
    /// <typeparam name="T">The type of symbol.</typeparam>
    /// <param name="symbol">The symbol to customize the help details for.</param>
    /// <param name="firstColumnText">A delegate to display the first help column (typically name and usage information).</param>
    /// <param name="secondColumnText">A delegate to display second help column (typically the description).</param>
    /// <param name="defaultValue">The displayed default value for the symbol.</param>
    /// <returns>The input symbol.</returns>
    /// <remarks>This should only be called once attached to a <see cref="CliCommand"/> which has been attached to the <see cref="CliRootCommand"/>.</remarks>
    public static T CustomizeHelp<T>(
        this T symbol,
        string? firstColumnText = null,
        string? secondColumnText = null,
        string? defaultValue = null)
        where T : CliSymbol
    {
        // get the root command
        if (GetRootCommand(symbol) is { } rootCommand
            && GetHelpAction(rootCommand) is { } helpAction)
        {
            helpAction.Builder.CustomizeSymbol(
                symbol,
                firstColumnText,
                secondColumnText,
                defaultValue);
        }

        return symbol;
    }

    /// <summary>
    /// Customizes the help for the symbol.
    /// </summary>
    /// <typeparam name="T">The type of symbol.</typeparam>
    /// <param name="symbol">The symbol to specify custom help details for.</param>
    /// <param name="firstColumnText">A delegate to display the first help column (typically name and usage information).</param>
    /// <param name="secondColumnText">A delegate to display second help column (typically the description).</param>
    /// <param name="defaultValue">A delegate to display the default value for the symbol.</param>
    /// <returns>The input symbol.</returns>
    /// <remarks>This should only be called once attached to a <see cref="CliCommand"/> which has been attached to the <see cref="CliRootCommand"/>.</remarks>
    public static T CustomizeHelp<T>(
        this T symbol,
        Func<Help.HelpContext, string?>? firstColumnText = null,
        Func<Help.HelpContext, string?>? secondColumnText = null,
        Func<Help.HelpContext, string?>? defaultValue = null)
        where T : CliSymbol
    {
        // get the root command
        if (GetRootCommand(symbol) is { } rootCommand
            && GetHelpAction(rootCommand) is { } helpAction)
        {
            helpAction.Builder.CustomizeSymbol(
                symbol,
                firstColumnText,
                secondColumnText,
                defaultValue);
        }

        return symbol;
    }

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
    }

    private static Help.HelpAction? GetHelpAction(CliCommand command)
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

    private static CliRootCommand? GetRootCommand(CliSymbol? symbol)
    {
        if (symbol is null)
        {
            return default;
        }

        if (symbol is CliRootCommand rootCommand)
        {
            return rootCommand;
        }

        foreach (var parent in symbol.Parents)
        {
            if (GetRootCommand(parent) is { } parentRootCommand)
            {
                return parentRootCommand;
            }
        }

        return default;
    }
}