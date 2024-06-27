// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleProgressBase.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The <see cref="AnsiConsoleProgress{T}"/> base.
/// </summary>
public abstract class AnsiConsoleProgressBase
{
    /// <summary>
    /// Initialises a new instance of the <see cref="AnsiConsoleProgressBase"/> class.
    /// </summary>
    private protected AnsiConsoleProgressBase()
    {
    }

    /// <summary>
    /// The progress statistics.
    /// </summary>
    protected static class ProgressStatics
    {
        /// <summary>
        /// The default context.
        /// </summary>
        public static readonly SynchronizationContext DefaultContext = new();
    }
}