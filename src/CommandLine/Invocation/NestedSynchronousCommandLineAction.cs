// -----------------------------------------------------------------------
// <copyright file="NestedSynchronousCommandLineAction.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

/// <summary>
/// A nested <see cref="SynchronousCommandLineAction"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NestedSynchronousCommandLineAction"/> class.
/// </remarks>
/// <param name="action">The <see cref="System.Action"/>.</param>
public class NestedSynchronousCommandLineAction(SynchronousCommandLineAction action) : SynchronousCommandLineAction, INestedCommandLineAction<SynchronousCommandLineAction>, INestedCommandLineAction
{
    private readonly Action<SynchronousCommandLineAction, ParseResult>? beforeInvoke;
    private readonly Action<SynchronousCommandLineAction, ParseResult>? afterInvoke;

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedSynchronousCommandLineAction"/> class.
    /// </summary>
    /// <param name="action">The <see cref="System.Action"/>.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    public NestedSynchronousCommandLineAction(SynchronousCommandLineAction action, Action<ParseResult> beforeInvoke, Action<ParseResult> afterInvoke)
        : this(action)
    {
        this.beforeInvoke = (_, parseResult) => beforeInvoke(parseResult);
        this.afterInvoke = (_, parseResult) => afterInvoke(parseResult);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedSynchronousCommandLineAction"/> class.
    /// </summary>
    /// <param name="action">The <see cref="System.Action"/>.</param>
    /// <param name="beforeInvoke">The action to call before invoking the nested action.</param>
    /// <param name="afterInvoke">The action to call after invoking the nested action.</param>
    internal NestedSynchronousCommandLineAction(SynchronousCommandLineAction action, Action<SynchronousCommandLineAction, ParseResult> beforeInvoke, Action<SynchronousCommandLineAction, ParseResult> afterInvoke)
        : this(action)
    {
        this.beforeInvoke = beforeInvoke;
        this.afterInvoke = afterInvoke;
    }

    /// <inheritdoc/>
    public SynchronousCommandLineAction Action { get; } = action;

    /// <inheritdoc/>
    CommandLineAction INestedCommandLineAction.Action => this.Action;

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