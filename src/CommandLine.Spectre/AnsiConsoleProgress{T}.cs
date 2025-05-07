// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleProgress{T}.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// Handler for <see cref="AnsiConsoleExtensions.Progress(IAnsiConsole)"/>.
/// </summary>
/// <typeparam name="T">The type of progress item.</typeparam>
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited", Justification = "Public API")]
public class AnsiConsoleProgress<T> : AnsiConsoleProgressBase, IProgress<T>
{
    private readonly SynchronizationContext synchronizationContext;
    private readonly Action<T>? handler;
    private readonly SendOrPostCallback invokeHandlers;
    private readonly Func<bool>? isComplete;

    /// <summary>
    /// Initialises a new instance of the <see cref="AnsiConsoleProgress{T}"/> class.
    /// </summary>
    /// <param name="handler">The message handler.</param>
    /// <param name="isComplete">The function to determine if the progress is complete.</param>
    internal AnsiConsoleProgress(Action<T> handler, Func<bool> isComplete)
        : this()
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(handler);
        this.handler = handler;
#else
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
#endif
        this.isComplete = isComplete;
    }

    private AnsiConsoleProgress()
    {
        // Capture the current synchronization context.
        // If there is no current context, we use a default instance targeting the ThreadPool.
        this.synchronizationContext = SynchronizationContext.Current ?? ProgressStatics.DefaultContext;
        this.invokeHandlers = state => this.handler?.Invoke((T)state!);
    }

    /// <summary>
    /// Gets a value indicating whether this is complete.
    /// </summary>
    public bool IsComplete => this.isComplete?.Invoke() ?? true;

    /// <inheritdoc/>
    void IProgress<T>.Report(T value) => this.OnReport(value);

    /// <summary>
    /// Reports a progress change.
    /// </summary>
    /// <param name="value">The value of the updated progress.</param>
    protected virtual void OnReport(T value)
    {
        // If there's no handler, don't bother going through the sync context.
        // Inside the callback, we'll need to check again, in case
        // an event handler is removed between now and then.
        if (this.handler is not null)
        {
            // Send the processing to the sync context.
            // (If T is a value type, it will get boxed here.)
            this.synchronizationContext.Send(this.invokeHandlers, value);
        }
    }
}