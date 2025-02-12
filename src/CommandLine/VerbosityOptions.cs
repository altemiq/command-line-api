// -----------------------------------------------------------------------
// <copyright file="VerbosityOptions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The verbosity options.
/// </summary>
[Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "This is required for parsing")]
[Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "This is required")]
public enum VerbosityOptions
{
    /// <summary>
    /// Quiet verbosity option.
    /// </summary>
    quiet,

    /// <summary>
    /// Short quiet verbosity option.
    /// </summary>
    q,

    /// <summary>
    /// Minimal verbosity option.
    /// </summary>
    minimal,

    /// <summary>
    /// Shor minimal verbosity option.
    /// </summary>
    m,

    /// <summary>
    /// Normal verbosity option.
    /// </summary>
    normal,

    /// <summary>
    /// Short normal verbosity option.
    /// </summary>
    n,

    /// <summary>
    /// Detailed verbosity option.
    /// </summary>
    detailed,

    /// <summary>
    /// Short detailed verbosity option.
    /// </summary>
    d,

    /// <summary>
    /// Diagnostic verbosity option.
    /// </summary>
    diagnostic,

    /// <summary>
    /// Short diagnostic verbosity option.
    /// </summary>
    diag,
}