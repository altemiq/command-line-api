// -----------------------------------------------------------------------
// <copyright file="CliConfigurationLogger.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

using Microsoft.Extensions.Logging;

/// <summary>
/// The <see cref="CliConfiguration"/> <see cref="ILogger"/>.
/// </summary>
/// <param name="configuration">The configuration.</param>
/// <param name="level">The log level.</param>
internal class CliConfigurationLogger(CliConfiguration configuration, LogLevel level) : ILogger
{
    /// <inheritdoc/>
    public IDisposable BeginScope<TState>(TState state) => throw new NotSupportedException();

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => logLevel >= level;

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!this.IsEnabled(logLevel))
        {
            return;
        }

        var writer = logLevel switch
        {
            LogLevel.Critical or LogLevel.Error => configuration.Error,
            _ => configuration.Output,
        };

        writer.WriteLine(formatter(state, exception));
        if (exception is not null)
        {
            writer.WriteLine(exception.ToString());
        }
    }
}