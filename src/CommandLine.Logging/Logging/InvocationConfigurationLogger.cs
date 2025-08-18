// -----------------------------------------------------------------------
// <copyright file="InvocationConfigurationLogger.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

/// <summary>
/// The <see cref="InvocationConfiguration"/> <see cref="ILogger"/>.
/// </summary>
/// <param name="configuration">The configuration.</param>
/// <param name="scopeProvider">The scope provider.</param>
internal sealed class InvocationConfigurationLogger(InvocationConfiguration configuration, IExternalScopeProvider? scopeProvider) : ILogger
{
    /// <inheritdoc/>
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => scopeProvider?.Push(state) ?? Internal.NullScope.Instance;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => logLevel is not LogLevel.None;

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
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