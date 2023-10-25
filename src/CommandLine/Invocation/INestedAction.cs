// -----------------------------------------------------------------------
// <copyright file="INestedAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// A nested <see cref="CliAction"/>.
/// </summary>
public interface INestedAction
{
    /// <summary>
    /// Gets the action.
    /// </summary>
    CliAction Action { get; }
}