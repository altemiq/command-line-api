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
    private readonly Action<SynchronousCliAction, ParseResult>? beforeInvoke;
    private readonly Action<SynchronousCliAction, ParseResult>? afterInvoke;

    /// <summary>
    /// Initialises a new instance of the <see cref="NestedSynchronousCliAction"/> class.
    /// </summary>
    /// <param name="action">The <see cref="CliAction"/>.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    public NestedSynchronousCliAction(SynchronousCliAction action, Action<ParseResult> beforeInvoke, Action<ParseResult> afterInvoke)
        : this(action)
    {
        this.beforeInvoke = (_, parseResult) => beforeInvoke(parseResult);
        this.afterInvoke = (_, parseResult) => afterInvoke(parseResult);
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="NestedSynchronousCliAction"/> class.
    /// </summary>
    /// <param name="action">The <see cref="CliAction"/>.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    internal NestedSynchronousCliAction(SynchronousCliAction action, Action<SynchronousCliAction, ParseResult> beforeInvoke, Action<SynchronousCliAction, ParseResult> afterInvoke)
        : this(action)
    {
        this.beforeInvoke = beforeInvoke;
        this.afterInvoke = afterInvoke;
    }

    /// <inheritdoc/>
    public SynchronousCliAction Action { get; } = action;

    /// <inheritdoc/>
    CliAction INestedAction.Action => this.Action;

    /// <inheritdoc/>
    public override int Invoke(ParseResult parseResult)
    {
        this.beforeInvoke?.Invoke(this, parseResult);
        try
        {
            return this.Action.Invoke(parseResult);
        }
        finally
        {
            this.afterInvoke?.Invoke(this, parseResult);
        }
    }
}