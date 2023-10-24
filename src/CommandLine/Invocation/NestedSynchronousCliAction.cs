// -----------------------------------------------------------------------
// <copyright file="NestedSynchronousCliAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// A nested <see cref="SynchronousCliAction"/>.
/// </summary>
/// <remarks>
/// Initialises a new instance of the <see cref="NestedSynchronousCliAction"/> class.
/// </remarks>
/// <param name="action">The <see cref="CliAction"/>.</param>
public class NestedSynchronousCliAction(SynchronousCliAction action) : SynchronousCliAction, INestedAction<SynchronousCliAction>, INestedAction
{
    /// <inheritdoc/>
    public SynchronousCliAction Action { get; } = action;

    /// <inheritdoc/>
    CliAction INestedAction.Action => this.Action;

    /// <inheritdoc/>
    public override int Invoke(ParseResult parseResult) => this.Action.Invoke(parseResult);
}