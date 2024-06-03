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
        var console = new TestConsole();

        var consoleProgress = AnsiConsoleProgress.Create(console, new AnsiConsoleProgressOptions { UpdateThreshold = 1 });

        UpdateProgress(consoleProgress);

        // wait for is complete to be true
        using var source = new CancellationTokenSource();
        source.CancelAfter(1000);
        await Task.Run(() =>
        {
            while (!consoleProgress.IsComplete)
            {
                Thread.Sleep(100);
            }
        }, source.Token);

        _ = consoleProgress.IsComplete.Should().Be(true);

        static void UpdateProgress(IProgress<AnsiConsoleProgressItem> progress)
        {
            progress.Report(new AnsiConsoleProgressItem("Simple Task", 0));
            progress.Report(new AnsiConsoleProgressItem("Indeterminate Task", -1));
            progress.Report(new AnsiConsoleProgressItem("Simple Task", 50));
            progress.Report(new AnsiConsoleProgressItem("Simple Task", 100));
            progress.Report(new AnsiConsoleProgressItem("Indeterminate Task", -1));
        }
    }
}
