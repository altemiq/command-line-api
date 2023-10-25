// -----------------------------------------------------------------------
// <copyright file="LoggingExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

/// <summary>
/// <see cref="Microsoft.Extensions.Logging"/> extensions.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Gets the log level.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <returns>The log level.</returns>
    public static Microsoft.Extensions.Logging.LogLevel GetLogLevel(this ParseResult parseResult) => parseResult.GetValue(CliOptions.VerbosityOption).GetLogLevel();

    private static Microsoft.Extensions.Logging.LogLevel GetLogLevel(this VerbosityOptions options) => options switch
    {
        VerbosityOptions.q or VerbosityOptions.quiet => Microsoft.Extensions.Logging.LogLevel.Error,
        VerbosityOptions.m or VerbosityOptions.minimal => Microsoft.Extensions.Logging.LogLevel.Warning,
        VerbosityOptions.n or VerbosityOptions.normal => Microsoft.Extensions.Logging.LogLevel.Information,
        VerbosityOptions.d or VerbosityOptions.detailed => Microsoft.Extensions.Logging.LogLevel.Debug,
        VerbosityOptions.diag or VerbosityOptions.diagnostic => Microsoft.Extensions.Logging.LogLevel.Trace,
        _ => throw new ArgumentOutOfRangeException(nameof(options)),
    };
}