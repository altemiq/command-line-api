// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleProgressItem.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

#if NET5_0_OR_GREATER
/// <summary>
/// The <see cref="IAnsiConsole"/> percentage item.
/// </summary>
/// <param name="Name">The name.</param>
/// <param name="Percentage">The percentage.</param>
public readonly record struct AnsiConsoleProgressItem(string Name, double Percentage);
#else
/// <summary>
/// The  <see cref="IAnsiConsole"/> percentage item.
/// </summary>
/// <param name="name">The name.</param>
/// <param name="percentage">The percentage.</param>
public readonly struct AnsiConsoleProgressItem(string name, double percentage)
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the percentage.
    /// </summary>
    public double Percentage { get; } = percentage;
}
#endif