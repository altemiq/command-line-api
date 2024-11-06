// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleProgressTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Spectre;

public class AnsiConsoleProgressTests
{
    [Fact]
    public async Task TestWithSimpleTasks()
    {
        TestConsole console = new();

        AnsiConsoleProgress<AnsiConsoleProgressItem> consoleProgress = AnsiConsoleProgress.Create(console, new AnsiConsoleProgressOptions { UpdateRate = TimeSpan.FromMilliseconds(1) });

        UpdateProgress(consoleProgress);

        // wait for is complete to be true
        using CancellationTokenSource source = new();
        source.CancelAfter(1000);
        await Task.Run(() =>
        {
            while (!consoleProgress.IsComplete)
            {
                Thread.Sleep(1);
            }
        }, source.Token);

        _ = consoleProgress.IsComplete.Should().BeTrue();
        _ = source.IsCancellationRequested.Should().BeFalse();

        static void UpdateProgress(IProgress<AnsiConsoleProgressItem> progress)
        {
            progress.Report(new AnsiConsoleProgressItem("Simple Task", 0));
            Thread.Sleep(1);
            progress.Report(new AnsiConsoleProgressItem("Indeterminate Task", double.PositiveInfinity));
            Thread.Sleep(1);
            progress.Report(new AnsiConsoleProgressItem("Simple Task", 50));
            Thread.Sleep(1);
            progress.Report(new AnsiConsoleProgressItem("Simple Task", 100));
            Thread.Sleep(1);
            progress.Report(new AnsiConsoleProgressItem("Indeterminate Task", double.NaN));
            Thread.Sleep(1);
        }
    }
}