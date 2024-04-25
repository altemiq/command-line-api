// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleProgress.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// Handler for <see cref="AnsiConsoleExtensions.Progress(IAnsiConsole)"/>.
/// </summary>
public static class AnsiConsoleProgress
{
    /// <summary>
    /// Creates the <see cref="Progress{T}" /> for <see cref="IAnsiConsole"/>.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="options">The optional options.</param>
    /// <returns>The progress class.</returns>
    public static AnsiConsoleProgress<AnsiConsoleProgressItem> Create(IAnsiConsole console, AnsiConsoleProgressOptions? options = default) => Create(console, Func<AnsiConsoleProgressItem>.Return, options);

    /// <summary>
    /// Creates the <see cref="IProgress{T}" /> for <see cref="IAnsiConsole"/>.
    /// </summary>
    /// <typeparam name="T">The type of progress item.</typeparam>
    /// <param name="console">The console.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="options">The optional options.</param>
    /// <returns>The progress class.</returns>
    public static AnsiConsoleProgress<T> Create<T>(IAnsiConsole console, Func<T, AnsiConsoleProgressItem> converter, AnsiConsoleProgressOptions? options = default)
    {
        options ??= AnsiConsoleProgressOptions.Default;
        return Create(() => EnsureColumns(console.Progress(), options), converter, options.UpdateThreshold);
    }

    /// <summary>
    /// Creates the <see cref="IProgress{T}" /> for <see cref="IAnsiConsole"/>.
    /// </summary>
    /// <typeparam name="T">The type of progress item.</typeparam>
    /// <param name="progress">The progress.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="options">The optional options.</param>
    /// <returns>The progress class.</returns>
    public static AnsiConsoleProgress<T> Create<T>(Progress progress, Func<T, AnsiConsoleProgressItem> converter, AnsiConsoleProgressOptions? options = default)
    {
        options ??= AnsiConsoleProgressOptions.Default;
        return Create(() => progress, converter, options.UpdateThreshold);
    }

    private static AnsiConsoleProgress<T> Create<T>(System.Func<Progress> progressFactory, Func<T, AnsiConsoleProgressItem> converter, int updatedThreshold = 1000)
    {
        var lastUpdate = DateTime.Now;
        var updateThreshold = TimeSpan.FromMilliseconds(updatedThreshold);
        ProgressContext? context = default;
        var progressTasks = new Dictionary<string, ProgressTask>(StringComparer.Ordinal);
        var contextLock = new object();

        return new AnsiConsoleProgress<T>(Handler, () => context is null);

        void Handler(T message)
        {
            var progressItem = converter(message);
            if (context is null)
            {
                lock (contextLock)
                {
                    if (context is null)
                    {
                        _ = Task.Run(() => progressFactory()
                            .Start(ctx =>
                            {
                                context = ctx;

                                // add the first task
                                var task = ctx.AddTask($"[green]{progressItem.Name}[/]");
                                task.IsIndeterminate = progressItem.Percentage < 0;

                                progressTasks.Add(progressItem.Name, task);

                                while (!progressTasks.Values.All(progressTask => progressTask.IsFinished))
                                {
                                    Thread.Sleep(100);
                                }

                                context = default;
                            }));

                        // wait for the context be valid
                        while (context is null)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            }

            if (!progressTasks.TryGetValue(progressItem.Name, out var progressTask))
            {
                progressTask = context.AddTask($"[green]{progressItem.Name}[/]");
                progressTasks.Add(progressItem.Name, progressTask);
            }

            var currentUpdate = DateTime.Now;
            if (currentUpdate - lastUpdate > updateThreshold)
            {
                lastUpdate = currentUpdate;
                if (progressItem.Percentage >= 0)
                {
                    progressTask.Value = progressItem.Percentage;
                }
            }

            if (progressItem.Percentage < 0D)
            {
                progressTask.StopTask();
            }
            else if (progressItem.Percentage >= 100D)
            {
                progressTask.Value = 100D;
                progressTask.StopTask();
            }
        }
    }

    private static Progress EnsureColumns(Progress progress, AnsiConsoleProgressOptions options)
    {
        var columnsToSet = new List<ProgressColumn>
        {
            new TaskDescriptionColumn(),
            new ProgressBarColumn(),
            new PercentageColumn(),
        };

        if (options.ShowRemainingTime)
        {
            columnsToSet.Add(new RemainingTimeColumn());
        }

        if (options.ShowSpinner)
        {
            columnsToSet.Add(new SpinnerColumn());
        }

        return progress.Columns([.. columnsToSet]);
    }

    private static class Func<T>
    {
        public static readonly Func<T, T> Return = new(_ => _);
    }
}