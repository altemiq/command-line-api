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
    /// <param name="configureTask">The action to perform to configure tasks.</param>
    /// <returns>The progress class.</returns>
    public static AnsiConsoleProgress<AnsiConsoleProgressItem> Create(IAnsiConsole console, AnsiConsoleProgressOptions? options = default, Action<string, ProgressTaskSettings>? configureTask = default) => Create(console, Func<AnsiConsoleProgressItem>.Return, options, configureTask);

    /// <summary>
    /// Creates the <see cref="IProgress{T}" /> for <see cref="IAnsiConsole"/>.
    /// </summary>
    /// <typeparam name="T">The type of progress item.</typeparam>
    /// <param name="console">The console.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="options">The optional options.</param>
    /// <param name="configureTask">The action to perform to configure tasks.</param>
    /// <returns>The progress class.</returns>
    public static AnsiConsoleProgress<T> Create<T>(IAnsiConsole console, Func<T, AnsiConsoleProgressItem> converter, AnsiConsoleProgressOptions? options = default, Action<string, ProgressTaskSettings>? configureTask = default)
    {
        options ??= AnsiConsoleProgressOptions.Default;
        return Create(() => EnsureColumns(console.Progress(), options), converter, options.UpdateRate, configureTask);
    }

    /// <summary>
    /// Creates the <see cref="IProgress{T}" /> for <see cref="IAnsiConsole"/>.
    /// </summary>
    /// <typeparam name="T">The type of progress item.</typeparam>
    /// <param name="progress">The progress.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="options">The optional options.</param>
    /// <param name="configureTask">The action to perform to configure tasks.</param>
    /// <returns>The progress class.</returns>
    public static AnsiConsoleProgress<T> Create<T>(Progress progress, Func<T, AnsiConsoleProgressItem> converter, AnsiConsoleProgressOptions? options = default, Action<string, ProgressTaskSettings>? configureTask = default)
    {
        options ??= AnsiConsoleProgressOptions.Default;
        return Create(() => progress, converter, options.UpdateRate, configureTask);
    }

    private static AnsiConsoleProgress<T> Create<T>(System.Func<Progress> progressFactory, Func<T, AnsiConsoleProgressItem> converter, TimeSpan updateRate, Action<string, ProgressTaskSettings>? configureTask)
    {
        const int ThreadUpdateRate = 100;
        var lastUpdate = DateTime.Now;
        ProgressContext? context = default;
        var progressTasks = new Collections.Concurrent.ConcurrentDictionary<string, ProgressTask>(StringComparer.Ordinal);
        var contextLock = new object();

        return new AnsiConsoleProgress<T>(Handler, () => context is null);

        void Handler(T message)
        {
            var progressItem = converter(message);
            EnsureContext(progressItem);
            var progressTask = AddOrUpdate(progressItem);
            UpdateProgress(progressTask);

            if (double.IsNaN(progressItem.Percentage))
            {
                progressTask.StopTask();
            }
            else if (progressItem.Percentage > progressTask.MaxValue && progressItem.Percentage < double.PositiveInfinity)
            {
                progressTask.Value = progressTask.MaxValue;
                progressTask.StopTask();
            }

            ProgressTask AddOrUpdate(AnsiConsoleProgressItem progressItem)
            {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER || NET472_OR_GREATER
                var processTask = progressTasks.AddOrUpdate(
                    progressItem.Name,
                    static (key, state) => Create(key, state.Context, state.ConfigureTask),
                    static (_, item, _) => item,
                    (Context: context!, ConfigureTask: configureTask));
#else
                var processTask = progressTasks.AddOrUpdate(
                    progressItem.Name,
                    key => Create(key, context!, configureTask),
                    (_, item) => item);
#endif

                processTask.IsIndeterminate = double.IsInfinity(progressItem.Percentage);

                return processTask;

                static ProgressTask Create(string key, ProgressContext context, Action<string, ProgressTaskSettings>? configureTask)
                {
                    var options = new ProgressTaskSettings();
                    if (configureTask is { } action)
                    {
                        action.Invoke(key, options);
                    }

                    return context.AddTask($"[green]{key}[/]", options);
                }
            }

            void EnsureContext(AnsiConsoleProgressItem progressItem)
            {
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
                                    _ = AddOrUpdate(progressItem);

                                    while (!progressTasks.Values.All(progressTask => progressTask.IsFinished))
                                    {
                                        Thread.Sleep(ThreadUpdateRate);
                                    }

                                    context = default;
                                }));

                            // wait for the context be valid
                            while (context is null)
                            {
                                Thread.Sleep(ThreadUpdateRate);
                            }
                        }
                    }
                }
            }

            void UpdateProgress(ProgressTask progressTask)
            {
                var currentUpdate = DateTime.Now;
                if (currentUpdate - lastUpdate > updateRate)
                {
                    lastUpdate = currentUpdate;
                    if (progressItem.Percentage is >= 0 and < double.PositiveInfinity)
                    {
                        progressTask.Value = progressItem.Percentage;
                    }
                }
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