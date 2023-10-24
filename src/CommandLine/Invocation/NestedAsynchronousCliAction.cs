// -----------------------------------------------------------------------
// <copyright file="NestedAsynchronousCliAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// A nested <see cref="AsynchronousCliAction"/>.
/// </summary>
/// <remarks>
/// Initialises a new instance of the <see cref="NestedAsynchronousCliAction"/> class.
/// </remarks>
/// <param name="action">The <see cref="CliAction"/>.</param>
public class NestedAsynchronousCliAction(AsynchronousCliAction action) : AsynchronousCliAction, INestedAction<AsynchronousCliAction>, INestedAction
{
    /// <inheritdoc/>
    public AsynchronousCliAction Action { get; } = action;

    /// <inheritdoc/>
    CliAction INestedAction.Action => this.Action;

    /// <inheritdoc/>
    public override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default) => this.Action.InvokeAsync(parseResult, cancellationToken);
}