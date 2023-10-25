// -----------------------------------------------------------------------
// <copyright file="CliConfigurationLoggerProvider.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

/// <summary>
/// The <see cref="CliConfigurationLogger"/> <see cref="ILoggerProvider"/>.
/// </summary>
/// <remarks>
/// Initialises a new instance of the <see cref="CliConfigurationLoggerProvider"/> class.
/// </remarks>
/// <param name="configuration">The configuration.</param>
internal sealed class CliConfigurationLoggerProvider(CliConfiguration configuration) : ILoggerProvider, ISupportExternalScope
{
    private readonly Collections.Concurrent.ConcurrentDictionary<string, CliConfigurationLogger> loggers = new();
    private readonly CliConfiguration configuration = configuration;

    private IExternalScopeProvider scopeProvider = Internal.NullExternalScopeProvider.Instance;

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) => this.loggers.GetOrAdd(categoryName, _ => new CliConfigurationLogger(this.configuration, this.scopeProvider));

    /// <inheritdoc/>
    public void Dispose() => GC.SuppressFinalize(this);

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => this.scopeProvider = scopeProvider;
}