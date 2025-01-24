// -----------------------------------------------------------------------
// <copyright file="HelpSymbolExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// <see cref="Help"/> <see cref="Symbol"/> extensions.
/// </summary>
public static class HelpSymbolExtensions
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
    /// <remarks>This should only be called once attached to a <see cref="Command"/> which has been attached to the <see cref="RootCommand"/>.</remarks>
    public static T CustomizeHelp<T>(
        this T symbol,
        string? firstColumnText = null,
        string? secondColumnText = null,
        string? defaultValue = null)
        where T : Symbol
    {
        // get the help action
        if (Internal.CommandLineActionHelpers.GetHelpAction(symbol) is { } helpAction)
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
    /// <remarks>This should only be called once attached to a <see cref="Command"/> which has been attached to the <see cref="RootCommand"/>.</remarks>
    public static T CustomizeHelp<T>(
        this T symbol,
        Func<Help.HelpContext, string?>? firstColumnText = null,
        Func<Help.HelpContext, string?>? secondColumnText = null,
        Func<Help.HelpContext, string?>? defaultValue = null)
        where T : Symbol
    {
        // get the help action
        if (Internal.CommandLineActionHelpers.GetHelpAction(symbol) is { } helpAction)
        {
            helpAction.Builder.CustomizeSymbol(
                symbol,
                firstColumnText,
                secondColumnText,
                defaultValue);
        }

        return symbol;
    }
}