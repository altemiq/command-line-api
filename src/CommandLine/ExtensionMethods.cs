// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// Extension methods.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Gets the value from the parse result or throws an exception.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="name">The name of the Symbol for which to get a value.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">Result from <paramref name="name"/> is <see langword="null"/>.</exception>
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static T GetRequiredValue<T>(this ParseResult parseResult, string name)
        where T : notnull => parseResult.GetValue<T>(name).ThrowIfNull();

    /// <summary>
    /// Gets the value from the parse result or throws an exception.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="argument">The argument for which to get a value.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">Result from <paramref name="argument"/> is <see langword="null"/>.</exception>
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static T GetRequiredValue<T>(this ParseResult parseResult, Argument<T> argument)
        where T : notnull => parseResult.GetValue(argument).ThrowIfNull();

    /// <summary>
    /// Gets the value from the parse result or throws an exception.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="option">The option for which to get a value.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">Result from <paramref name="option"/> is <see langword="null"/>.</exception>
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static T GetRequiredValue<T>(this ParseResult parseResult, Option<T> option)
        where T : notnull => parseResult.GetValue(option).ThrowIfNull();

    /// <summary>
    /// Gets the result for the specified argument.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="argument">The argument for which to find a result.</param>
    /// <returns>A result for the specified argument.</returns>
    /// <exception cref="ArgumentNullException">Failed to find the result.</exception>
    public static Parsing.ArgumentResult GetRequiredResult(this ParseResult parseResult, Argument argument) => parseResult.GetResult(argument).ThrowIfNull();

    /// <summary>
    /// Gets the result for the specified command.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="command">The command for which to find a result.</param>
    /// <returns>A result for the specified command.</returns>
    /// <exception cref="ArgumentNullException">Failed to find the result.</exception>
    public static Parsing.CommandResult GetRequiredResult(this ParseResult parseResult, Command command) => parseResult.GetResult(command).ThrowIfNull();

    /// <summary>
    /// Gets the result for the specified option.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="option">The option for which to find a result.</param>
    /// <returns>A result for the specified option.</returns>
    /// <exception cref="ArgumentNullException">Failed to find the result.</exception>
    public static Parsing.OptionResult GetRequiredResult(this ParseResult parseResult, Option option) => parseResult.GetResult(option).ThrowIfNull();

    /// <summary>
    /// Gets the result for the specified directive.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="directive">The directive for which to find a result.</param>
    /// <returns>A result for the specified directive.</returns>
    /// <exception cref="ArgumentNullException">Failed to find the result.</exception>
    public static Parsing.DirectiveResult GetRequiredResult(this ParseResult parseResult, Directive directive) => parseResult.GetResult(directive).ThrowIfNull();

    /// <summary>
    /// Gets the result for the specified symbol.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="symbol">The symbol for which to find a result.</param>
    /// <returns>A result for the specified symbol.</returns>
    /// <exception cref="ArgumentNullException">Failed to find the result.</exception>
    public static Parsing.SymbolResult GetRequiredResult(this ParseResult parseResult, Symbol symbol) => parseResult.GetResult(symbol).ThrowIfNull();

    /// <summary>
    /// Gets the command, if any, from the symbol result.
    /// </summary>
    /// <param name="symbolResult">The symbol result.</param>
    /// <returns>A command for the specified symbol, or <see langword="null"/> if it was not provided and no default was configured.</returns>
    public static Command? GetCommand(this Parsing.SymbolResult symbolResult)
    {
        return GetCommandCore(symbolResult);

        static Command? GetCommandCore(Parsing.SymbolResult? symbolResult)
        {
            return symbolResult switch
            {
                { Parent: Parsing.CommandResult commandResult } => commandResult.Command,
                { Parent: Parsing.SymbolResult parentSymbolResult } => GetCommandCore(parentSymbolResult),
                Parsing.ArgumentResult { Argument: { } argument } => GetCommandFromSymbol(argument),
                Parsing.OptionResult { Option: { } option } => GetCommandFromSymbol(option),
                _ => default,
            };

            static Command? GetCommandFromSymbol(Symbol symbol)
            {
                foreach (var parent in symbol.Parents)
                {
                    if (parent is Command command)
                    {
                        return command;
                    }

                    if (parent is Symbol parentSymbol && GetCommandFromSymbol(parentSymbol) is { } parentCommand)
                    {
                        return parentCommand;
                    }
                }

                return default;
            }
        }
    }

    /// <summary>
    /// Gets the command from the symbol result.
    /// </summary>
    /// <param name="symbolResult">The symbol result.</param>
    /// <returns>A command for the specified symbol.</returns>
    /// <exception cref="ArgumentNullException">Failed to find the result.</exception>
    public static Command GetRequiredCommand(this Parsing.SymbolResult symbolResult) => symbolResult.GetCommand().ThrowIfNull();

    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static T ThrowIfNull<T>(this T? value)
        where T : notnull => value ?? throw new ArgumentNullException(nameof(value));
}