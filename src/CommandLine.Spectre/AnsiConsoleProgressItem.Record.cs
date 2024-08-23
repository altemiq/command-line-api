// -----------------------------------------------------------------------
// <copyright file="AnsiConsoleProgressItem.Record.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The <see cref="IAnsiConsole"/> percentage item.
/// </summary>
/// <param name="Name">The name.</param>
/// <param name="Percentage">
///   <para>The percentage.</para>
///   <list type="table">
///     <listheader>
///       <term>Value</term>
///       <description>Meaning</description>
///     </listheader>
///     <item>
///       <term><see cref="double.PositiveInfinity"/></term>
///       <description>Sets <see cref="ProgressTask.IsIndeterminate"/> to <see langword="true"/></description>
///     </item>
///     <item>
///       <term><see cref="double.NaN"/></term>
///       <description>Comletes the <see cref="ProgressTask"/> without setting <see cref="ProgressTask.Value"/>.</description>
///     </item>
///     <item>
///       <term>greater than or equal to 100</term>
///       <description>Completes the <see cref="ProgressTask"/> and sets <see cref="ProgressTask.Value"/> to 100%.</description>
///     </item>
///   </list>
/// </param>
public readonly record struct AnsiConsoleProgressItem(string Name, double Percentage);