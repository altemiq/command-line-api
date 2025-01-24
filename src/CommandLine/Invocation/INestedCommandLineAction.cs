// -----------------------------------------------------------------------
// <copyright file="INestedCommandLineAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// A nested <see cref="Action"/>.
/// </summary>
public interface INestedCommandLineAction
{
    /// <summary>
    /// Gets the action.
    /// </summary>
    CommandLineAction Action { get; }
}