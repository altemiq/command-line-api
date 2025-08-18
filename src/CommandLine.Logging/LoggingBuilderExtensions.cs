// -----------------------------------------------------------------------
// <copyright file="LoggingBuilderExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130, CheckNamespace
namespace Microsoft.Extensions.Logging;
#pragma warning restore IDE0130, CheckNamespace

using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// The <see cref="ILoggingBuilder"/> extensions.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="System.CommandLine.InvocationConfiguration"/> as a provider.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The logging builder.</returns>
    public static ILoggingBuilder AddCommandLineConfiguration(this ILoggingBuilder builder, System.CommandLine.InvocationConfiguration configuration)
    {
        builder.Services.Add(DependencyInjection.ServiceDescriptor.Singleton(configuration));
        builder.Services.TryAddEnumerable(DependencyInjection.ServiceDescriptor.Singleton<ILoggerProvider, System.CommandLine.Logging.InvocationConfigurationLoggerProvider>());
        return builder;
    }
}