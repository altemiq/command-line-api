// -----------------------------------------------------------------------
// <copyright file="HelpConfigurationExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// <see cref="Help"/> <see cref="CliConfiguration"/> extensions.
/// </summary>
public static class HelpConfigurationExtensions
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
        if (Internal.CommandHelpers.GetRootCommand(symbol) is { } rootCommand
            && Internal.ActionHelpers.GetHelpAction(rootCommand) is { } helpAction)
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
        if (Internal.CommandHelpers.GetRootCommand(symbol) is { } rootCommand
            && Internal.ActionHelpers.GetHelpAction(rootCommand) is { } helpAction)
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
    /// Configures the help.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure function.</param>
    /// <returns>The configuration for chaining.</returns>
    public static T ConfigureHelp<T>(this T configuration, Action<Help.HelpBuilder> configure)
        where T : CliConfiguration
    {
        _ = configuration.RootCommand.ConfigureHelp(configure);
        return configuration;
    }
}