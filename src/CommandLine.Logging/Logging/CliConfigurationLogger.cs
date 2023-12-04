// -----------------------------------------------------------------------
// <copyright file="CliConfigurationLogger.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

/// <summary>
/// The <see cref="CliConfiguration"/> <see cref="ILogger"/>.
/// </summary>
/// <param name="configuration">The configuration.</param>
/// <param name="scopeProvider">The scope provider.</param>
internal sealed class CliConfigurationLogger(CliConfiguration configuration, IExternalScopeProvider? scopeProvider) : ILogger
{
    /// <summary>
    /// Gets or sets the scope provider.
    /// </summary>
    internal IExternalScopeProvider? ScopeProvider { get; set; } = scopeProvider;

    /// <inheritdoc/>
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => this.ScopeProvider?.Push(state) ?? Internal.NullScope.Instance;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

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