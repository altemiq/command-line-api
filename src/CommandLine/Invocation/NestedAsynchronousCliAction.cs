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
    private readonly Func<AsynchronousCliAction, ParseResult, CancellationToken, Task>? beforeInvoke;
    private readonly Func<AsynchronousCliAction, ParseResult, CancellationToken, Task>? afterInvoke;

    /// <summary>
    /// Initialises a new instance of the <see cref="NestedAsynchronousCliAction"/> class.
    /// </summary>
    /// <param name="action">The <see cref="CliAction"/>.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    public NestedAsynchronousCliAction(AsynchronousCliAction action, Func<ParseResult, CancellationToken, Task> beforeInvoke, Func<ParseResult, CancellationToken, Task> afterInvoke)
        : this(action)
    {
        this.beforeInvoke = (_, parseResult, cancellationToken) => beforeInvoke(parseResult, cancellationToken);
        this.afterInvoke = (_, parseResult, cancellationToken) => afterInvoke(parseResult, cancellationToken);
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="NestedAsynchronousCliAction"/> class.
    /// </summary>
    /// <param name="action">The <see cref="CliAction"/>.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    internal NestedAsynchronousCliAction(AsynchronousCliAction action, Func<AsynchronousCliAction, ParseResult, CancellationToken, Task> beforeInvoke, Func<AsynchronousCliAction, ParseResult, CancellationToken, Task> afterInvoke)
        : this(action)
    {
        this.beforeInvoke = beforeInvoke;
        this.afterInvoke = afterInvoke;
    }

    /// <inheritdoc/>
    public AsynchronousCliAction Action { get; } = action;

    /// <inheritdoc/>
    CliAction INestedAction.Action => this.Action;

    /// <inheritdoc/>
    public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
    {
        if (this.beforeInvoke is { } beforeInvokeFunc)
        {
            await beforeInvokeFunc(this, parseResult, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            return await this.Action.InvokeAsync(parseResult, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (this.afterInvoke is { } afterInvokeFunc)
            {
                await afterInvokeFunc(this, parseResult, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}