// -----------------------------------------------------------------------
// <copyright file="ParseResultExtensions.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The parse result extensions.
/// </summary>
public static class ParseResultExtensions
{
    /// <summary>
    /// Creates the <see cref="IAnsiConsole"/> instance.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="outputOption">The output option.</param>
    /// <returns>The ANSI console.</returns>
    public static IAnsiConsole CreateConsole(this ParseResult? parseResult, Option<FileInfo>? outputOption = default)
    {
        if (parseResult is not null)
        {
            var (ansiSupport, writer) = outputOption is { } option && parseResult.GetValue(option) is { } outputFile
                ? (AnsiSupport.No, outputFile.CreateText())
                : (GetAnsiSupport(parseResult), parseResult.Configuration.Output);

            return global::Spectre.Console.AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = ansiSupport,
                ColorSystem = ColorSystemSupport.Detect,
                Out = new AnsiConsoleOutput(writer),
            });
        }

        return global::Spectre.Console.AnsiConsole.Console;

        static AnsiSupport GetAnsiSupport(ParseResult parseResult)
        {
            return parseResult.Configuration.Output == Console.Out ? AnsiSupport.Detect : AnsiSupport.No;
        }
    }
}