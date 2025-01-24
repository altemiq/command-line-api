// -----------------------------------------------------------------------
// <copyright file="INestedCommandLineAction{T}.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// A nested <see cref="System.Action"/>.
/// </summary>
/// <typeparam name="T">The action type.</typeparam>
public interface INestedCommandLineAction<out T>
    where T : CommandLineAction
{
    /// <summary>
    /// Gets the action.
    /// </summary>
    T Action { get; }
}