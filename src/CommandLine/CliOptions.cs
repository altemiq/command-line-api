// -----------------------------------------------------------------------
// <copyright file="CliOptions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The <see cref="CliOption"/> values.
/// </summary>
public static class CliOptions
{
    /// <summary>
    /// The verbosity option.
    /// </summary>
    public static readonly CliOption<VerbosityOptions> VerbosityOption = new("--verbosity", "-v")
    {
        HelpName = "LEVEL",
        DefaultValueFactory = _ => VerbosityOptions.normal,
        Description = "Set the verbosity level. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].",
        Recursive = true,
    };
}