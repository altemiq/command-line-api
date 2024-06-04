// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleProgressOptions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The <see cref="AnsiConsoleProgress"/> options.
/// </summary>
public class AnsiConsoleProgressOptions
{
    /// <summary>
    /// The default settings.
    /// </summary>
    public static readonly AnsiConsoleProgressOptions Default = new();

    /// <summary>
    /// Gets or sets the update rate.
    /// Defaults to once a second.
    /// </summary>
    public TimeSpan UpdateRate { get; set; } = TimeSpan.FromSeconds(1D);

    /// <summary>
    /// Gets or sets a value indicating whether to show the <see cref="RemainingTimeColumn"/>.
    /// </summary>
    /// <remarks>The only operates on a <see cref="IAnsiConsole"/> instance. When passing a <see cref="Progress"/> this property is ignored.</remarks>
    public bool ShowRemainingTime { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show the <see cref="SpinnerColumn"/>.
    /// </summary>
    /// <remarks>The only operates on a <see cref="IAnsiConsole"/> instance. When passing a <see cref="Progress"/> this property is ignored.</remarks>
    public bool ShowSpinner { get; set; } = true;
}