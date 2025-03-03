// -----------------------------------------------------------------------
// <copyright file="CommandLineConfigurationLoggerProvider.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging;

/// <summary>
/// The <see cref="CommandLineConfigurationLogger"/> <see cref="ILoggerProvider"/>.
/// </summary>
/// <remarks>
/// Initialises a new instance of the <see cref="CommandLineConfigurationLoggerProvider"/> class.
/// </remarks>
/// <param name="configuration">The configuration.</param>
/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing category names.</param>
[ProviderAlias(nameof(CommandLineConfiguration))]
internal sealed class CommandLineConfigurationLoggerProvider(CommandLineConfiguration configuration, IEqualityComparer<string> comparer) : ILoggerProvider, ISupportExternalScope
{
    private readonly Collections.Concurrent.ConcurrentDictionary<string, CommandLineConfigurationLogger> loggers = new(comparer);
    private readonly CommandLineConfiguration configuration = configuration;
    private IExternalScopeProvider scopeProvider = Internal.NullExternalScopeProvider.Instance;

    /// <summary>
    /// Initialises a new instance of the <see cref="CommandLineConfigurationLoggerProvider"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public CommandLineConfigurationLoggerProvider(CommandLineConfiguration configuration)
        : this(configuration, StringComparer.Ordinal)
    {
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) =>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER || NET472_OR_GREATER
        this.loggers.GetOrAdd(categoryName, static (_, values) => new CommandLineConfigurationLogger(values.configuration, values.scopeProvider), (this.configuration, this.scopeProvider));
#else
        this.loggers.GetOrAdd(categoryName, _ => new CommandLineConfigurationLogger(this.configuration, this.scopeProvider));
#endif

    /// <inheritdoc/>
    public void Dispose() => GC.SuppressFinalize(this);

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => this.scopeProvider = scopeProvider;
}