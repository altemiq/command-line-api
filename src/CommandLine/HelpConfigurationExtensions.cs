// -----------------------------------------------------------------------
// <copyright file="HelpConfigurationExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// <see cref="Help"/> <see cref="CommandLineConfiguration"/> extensions.
/// </summary>
public static class HelpConfigurationExtensions
{
    /// <summary>
    /// Configures the help.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure function.</param>
    /// <returns>The configuration for chaining.</returns>
    public static T ConfigureHelp<T>(this T configuration, Action<Help.HelpBuilder> configure)
        where T : CommandLineConfiguration
    {
        _ = configuration.RootCommand.ConfigureHelp(configure);
        return configuration;
    }
}