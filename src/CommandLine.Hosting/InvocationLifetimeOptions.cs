// -----------------------------------------------------------------------
// <copyright file="InvocationLifetimeOptions.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

/// <summary>
/// The <see cref="InvocationLifetime"/> options.
/// </summary>
#pragma warning disable S2094 // Classes should not be empty
public class InvocationLifetimeOptions : Microsoft.Extensions.Hosting.ConsoleLifetimeOptions;
#pragma warning restore S2094 // Classes should not be empty