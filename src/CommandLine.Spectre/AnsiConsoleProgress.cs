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
    public static AnsiConsoleProgress<AnsiConsoleProgressItem> Create(IAnsiConsole console, AnsiConsoleProgressOptions? options = default, Action<string, ProgressTaskSettings>? configureTask = default) =>
        Create(console, Func<AnsiConsoleProgressItem>.Return, options, configureTask);

    /// <summary>
    /// Creates the <see cref="IProgress{T}" /> for <see cref="IAnsiConsole"/>.
    /// </summary>
    /// <typeparam name="T">The type of progress item.</typeparam>
    /// <param name="console">The console.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="options">The optional options.</param>
    /// <param name="configureTask">The action to perform to configure tasks.</param>
    /// <returns>The progress class.</returns>
    public static AnsiConsoleProgress<T> Create<T>(IAnsiConsole console, Func<T, AnsiConsoleProgressItem> converter, AnsiConsoleProgressOptions? options = default, Action<string, ProgressTaskSettings>? configureTask = default) =>
        Create(options ?? AnsiConsoleProgressOptions.Default, o => EnsureColumns(console.Progress(), o), converter, configureTask);

    /// <summary>
    /// Creates the <see cref="IProgress{T}" /> for <see cref="IAnsiConsole"/>.
    /// </summary>
    /// <typeparam name="T">The type of progress item.</typeparam>
    /// <param name="progress">The progress.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="options">The optional options.</param>
    /// <param name="configureTask">The action to perform to configure tasks.</param>
    /// <returns>The progress class.</returns>
    public static AnsiConsoleProgress<T> Create<T>(Progress progress, Func<T, AnsiConsoleProgressItem> converter, AnsiConsoleProgressOptions? options = default, Action<string, ProgressTaskSettings>? configureTask = default) =>
        Create(options ?? AnsiConsoleProgressOptions.Default, _ => progress, converter, configureTask);

    private static AnsiConsoleProgress<T> Create<T>(AnsiConsoleProgressOptions options, Func<AnsiConsoleProgressOptions, Progress> progressFactory, Func<T, AnsiConsoleProgressItem> converter, Action<string, ProgressTaskSettings>? configureTask)
    {
        const int ThreadUpdateRate = 100;
        var updateRate = options.UpdateRate;
        var lastUpdate = DateTime.UtcNow;
        ProgressContext? context = default;
        var progressTasks = new Collections.Concurrent.ConcurrentDictionary<string, ProgressTask>(StringComparer.Ordinal);
        var contextLock =
#if NET9_0_OR_GREATER
            new Lock();
#else
            new object();
#endif

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

            ProgressTask AddOrUpdate(AnsiConsoleProgressItem ansiConsoleProgressItem)
            {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER || NET472_OR_GREATER
                var processTask = progressTasks.AddOrUpdate(
                    ansiConsoleProgressItem.Name,
                    static (key, state) => CreateProgressTask(key, state.Context, state.ConfigureTask),
                    static (_, item, _) => item,
                    (Context: context!, ConfigureTask: configureTask));
#else
                var processTask = progressTasks.AddOrUpdate(
                    ansiConsoleProgressItem.Name,
                    key => CreateProgressTask(key, context!, configureTask),
                    (_, item) => item);
#endif

                processTask.IsIndeterminate = double.IsInfinity(ansiConsoleProgressItem.Percentage);

                return processTask;

                static ProgressTask CreateProgressTask(string key, ProgressContext context, Action<string, ProgressTaskSettings>? configureTask)
                {
                    var options = new ProgressTaskSettings();
                    configureTask?.Invoke(key, options);
                    return context.AddTask($"[green]{key}[/]", options);
                }
            }

            void EnsureContext(AnsiConsoleProgressItem ansiConsoleProgressItem)
            {
                if (context is not null)
                {
                    return;
                }

                lock (contextLock)
                {
                    if (context is not null)
                    {
                        return;
                    }

                    _ = Task.Run(() => progressFactory(options)
                        .Start(ctx =>
                        {
                            context = ctx;

                            // add the first task
                            _ = AddOrUpdate(ansiConsoleProgressItem);

                            while (!progressTasks.Values.All(static progressTask => progressTask.IsFinished))
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

            void UpdateProgress(ProgressTask progressTaskToUpdate)
            {
                var currentUpdate = DateTime.UtcNow;
                if (currentUpdate - lastUpdate <= updateRate)
                {
                    return;
                }

                lastUpdate = currentUpdate;
                if (progressItem.Percentage is >= 0 and < double.PositiveInfinity)
                {
                    progressTaskToUpdate.Value = progressItem.Percentage;
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
        public static readonly Func<T, T> Return = static x => x;
    }
}