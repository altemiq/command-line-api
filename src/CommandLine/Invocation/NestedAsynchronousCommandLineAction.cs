// -----------------------------------------------------------------------
// <copyright file="NestedAsynchronousCommandLineAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// A nested <see cref="AsynchronousCommandLineAction"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NestedAsynchronousCommandLineAction"/> class.
/// </remarks>
/// <param name="action">The <see cref="CommandLineAction"/>.</param>
public class NestedAsynchronousCommandLineAction(AsynchronousCommandLineAction action) : AsynchronousCommandLineAction, INestedCommandLineAction<AsynchronousCommandLineAction>, INestedCommandLineAction
{
    private readonly Func<AsynchronousCommandLineAction, ParseResult, CancellationToken, Task>? beforeInvoke;
    private readonly Func<AsynchronousCommandLineAction, ParseResult, CancellationToken, Task>? afterInvoke;

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedAsynchronousCommandLineAction"/> class.
    /// </summary>
    /// <param name="action">The <see cref="CommandLineAction"/>.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    public NestedAsynchronousCommandLineAction(AsynchronousCommandLineAction action, Func<ParseResult, CancellationToken, Task> beforeInvoke, Func<ParseResult, CancellationToken, Task> afterInvoke)
        : this(action)
    {
        this.beforeInvoke = (_, parseResult, cancellationToken) => beforeInvoke(parseResult, cancellationToken);
        this.afterInvoke = (_, parseResult, cancellationToken) => afterInvoke(parseResult, cancellationToken);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedAsynchronousCommandLineAction"/> class.
    /// </summary>
    /// <param name="action">The <see cref="System.Action"/>.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    internal NestedAsynchronousCommandLineAction(AsynchronousCommandLineAction action, Func<AsynchronousCommandLineAction, ParseResult, CancellationToken, Task> beforeInvoke, Func<AsynchronousCommandLineAction, ParseResult, CancellationToken, Task> afterInvoke)
        : this(action)
    {
        this.beforeInvoke = beforeInvoke;
        this.afterInvoke = afterInvoke;
    }

    /// <inheritdoc/>
    public AsynchronousCommandLineAction Action { get; } = action;

    /// <inheritdoc/>
    CommandLineAction INestedCommandLineAction.Action => this.Action;

    /// <inheritdoc/>
    public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
    {
        if (this.beforeInvoke is not null)
        {
            await this.beforeInvoke(this, parseResult, cancellationToken).ConfigureAwait(false);
        }

        try
        {
            return await this.Action.InvokeAsync(parseResult, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (this.afterInvoke is not null)
            {
                await this.afterInvoke(this, parseResult, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}