// -----------------------------------------------------------------------
// <copyright file="InvocationConfigurationLoggerProvider.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

/// <summary>
/// The <see cref="InvocationConfigurationLogger"/> <see cref="ILoggerProvider"/>.
/// </summary>
/// <param name="configuration">The configuration.</param>
/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing category names.</param>
[ProviderAlias(nameof(InvocationConfiguration))]
internal sealed class InvocationConfigurationLoggerProvider(InvocationConfiguration configuration, IEqualityComparer<string> comparer) : ILoggerProvider, ISupportExternalScope
{
    private readonly Collections.Concurrent.ConcurrentDictionary<string, InvocationConfigurationLogger> loggers = new(comparer);
    private IExternalScopeProvider externalScopeProvider = Internal.NullExternalScopeProvider.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvocationConfigurationLoggerProvider"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public InvocationConfigurationLoggerProvider(InvocationConfiguration configuration)
        : this(configuration, StringComparer.Ordinal)
    {
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER || NET472_OR_GREATER
        this.loggers.GetOrAdd(categoryName, static (_, values) => new InvocationConfigurationLogger(values.configuration, values.scopeProvider), (configuration, scopeProvider: this.externalScopeProvider));
#else
        this.loggers.GetOrAdd(categoryName, _ => new InvocationConfigurationLogger(configuration, this.externalScopeProvider));
#endif

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => this.externalScopeProvider = scopeProvider;
}