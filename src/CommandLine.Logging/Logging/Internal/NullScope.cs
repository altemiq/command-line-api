// -----------------------------------------------------------------------
// <copyright file="NullScope.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Logging.Internal;

/// <summary>
/// An empty scope without any logic.
/// </summary>
internal sealed class NullScope : IDisposable
{
    private NullScope()
    {
    }

    /// <summary>
    /// Gets a cached instance of <see cref="NullScope"/>.
    /// </summary>
    public static NullScope Instance { get; } = new();

    /// <inheritdoc />
    public void Dispose()
    {
    }
}