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
/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing category names.</param>
[ProviderAlias(nameof(CliConfiguration))]
internal sealed class CliConfigurationLoggerProvider(CliConfiguration configuration, IEqualityComparer<string> comparer) : ILoggerProvider, ISupportExternalScope
{
    private readonly Collections.Concurrent.ConcurrentDictionary<string, CliConfigurationLogger> loggers = new(comparer);
    private readonly CliConfiguration configuration = configuration;
    private IExternalScopeProvider scopeProvider = Internal.NullExternalScopeProvider.Instance;

    /// <summary>
    /// Initialises a new instance of the <see cref="CliConfigurationLoggerProvider"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public CliConfigurationLoggerProvider(CliConfiguration configuration)
        : this(configuration, StringComparer.Ordinal)
    {
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) => this.loggers.GetOrAdd(categoryName, _ => new CliConfigurationLogger(this.configuration, this.scopeProvider));

    /// <inheritdoc/>
    public void Dispose() => GC.SuppressFinalize(this);

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => this.scopeProvider = scopeProvider;
}