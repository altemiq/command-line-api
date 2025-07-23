// -----------------------------------------------------------------------
// <copyright file="NullExternalScopeProvider.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging.Internal;

/// <summary>
/// Scope provider that does nothing.
/// </summary>
internal sealed class NullExternalScopeProvider : IExternalScopeProvider
{
    private NullExternalScopeProvider()
    {
    }

    /// <summary>
    /// Gets a cached instance of <see cref="NullExternalScopeProvider"/>.
    /// </summary>
    public static NullExternalScopeProvider Instance { get; } = new();

    /// <inheritdoc />
    void IExternalScopeProvider.ForEachScope<TState>(Action<object?, TState> callback, TState state)
    {
    }

    /// <inheritdoc />
    IDisposable IExternalScopeProvider.Push(object? state) => NullScope.Instance;
}