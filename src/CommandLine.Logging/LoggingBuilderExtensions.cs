// -----------------------------------------------------------------------
// <copyright file="LoggingBuilderExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// The <see cref="ILoggingBuilder"/> extensions.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="System.CommandLine.CliConfiguration"/> as a provider.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The logging builder.</returns>
    public static ILoggingBuilder AddCliConfiguration(this ILoggingBuilder builder, System.CommandLine.CliConfiguration configuration)
    {
        builder.Services.Add(ServiceDescriptor.Singleton(configuration));
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, System.CommandLine.Logging.CliConfigurationLoggerProvider>());
        return builder;
    }
}