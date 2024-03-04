// -----------------------------------------------------------------------
// <copyright file="HostingExtensions.ApplicationHost.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#if NET7_0_OR_GREATER
namespace System.CommandLine.Hosting;

using System.CommandLine.Invocation;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;

/// <content>
/// The <see cref="Microsoft.Extensions.Hosting.HostApplicationBuilder"/> extensions.
/// </content>
public static partial class HostingExtensions
{
    /// <summary>
    /// Use the application host builder to create a host.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configureHost">The function to configure the host.</param>
    /// <returns>The configuration for chaining.</returns>
    public static T UseApplicationHost<T>(this T configuration, Action<ParseResult?, Microsoft.Extensions.Hosting.HostApplicationBuilder>? configureHost = default)
        where T : CliConfiguration => UseApplicationHost(configuration, Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder, configureHost);

    /// <summary>
    /// Use the application host builder to create a host.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configureHost">The function to configure the host.</param>
    /// <returns>The configuration for chaining.</returns>
    public static T UseApplicationHost<T>(this T configuration, Func<string[], Microsoft.Extensions.Hosting.HostApplicationBuilder> hostBuilderFactory, Action<ParseResult?, Microsoft.Extensions.Hosting.HostApplicationBuilder>? configureHost = default)
        where T : CliConfiguration
    {
        if (configuration.RootCommand is CliRootCommand root)
        {
            root.Add(new CliDirective(HostingDirectiveName));
        }

        BuilderAction.SetHandlers(
            configuration.RootCommand,
            parseResult =>
            {
                var hostBuilder = hostBuilderFactory(parseResult?.UnmatchedTokens.ToArray() ?? []);
                if (parseResult is not null)
                {
                    _ = hostBuilder.Services.AddSingleton(parseResult);

                    if (configuration.RootCommand is CliRootCommand root
                        && root.Directives.SingleOrDefault(d => string.Equals(d.Name, HostingDirectiveName, StringComparison.Ordinal)) is { } directive
                        && parseResult.GetResult(directive) is { } directiveResult)
                    {
                        hostBuilder.Configuration.Sources.Add(
                            new MemoryConfigurationSource()
                            {
                                InitialData = directiveResult.Values.Select(s =>
                                {
                                    var parts = s.Split(Separator, count: 2);
                                    var key = parts[0];
                                    var value = parts.Length > 1 ? parts[1] : null;
                                    return new KeyValuePair<string, string?>(key, value);
                                }).ToList(),
                            });
                    }
                }

                _ = hostBuilder.UseInvocationLifetime();
                return hostBuilder;
            },
            builder => builder.Build(),
            (parseResult, builder) => configureHost?.Invoke(parseResult, builder),
            (_, host, cancellationToken) => host.StartAsync(cancellationToken),
            (_, host, cancellationToken) => host.StopAsync(cancellationToken));

        return configuration;
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Use the invocation lifetime.
    /// </summary>
    /// <param name="host">The host builder.</param>
    /// <param name="configureOptions">The options to configure.</param>
    /// <returns>The host builder for chaining.</returns>
    public static Microsoft.Extensions.Hosting.IHostApplicationBuilder UseInvocationLifetime(this Microsoft.Extensions.Hosting.IHostApplicationBuilder host, Action<InvocationLifetimeOptions>? configureOptions = null)
    {
        _ = host.Services.AddSingleton<Microsoft.Extensions.Hosting.IHostLifetime, InvocationLifetime>();
        if (configureOptions is { } configureOptionsAction)
        {
            _ = host.Services.Configure(configureOptionsAction);
        }

        return host;
    }
#else
    /// <summary>
    /// Use the invocation lifetime.
    /// </summary>
    /// <param name="host">The host builder.</param>
    /// <param name="configureOptions">The options to configure.</param>
    /// <returns>The host builder for chaining.</returns>
    public static Microsoft.Extensions.Hosting.HostApplicationBuilder UseInvocationLifetime(this Microsoft.Extensions.Hosting.HostApplicationBuilder host, Action<InvocationLifetimeOptions>? configureOptions = null)
    {
        _ = host.Services.AddSingleton<Microsoft.Extensions.Hosting.IHostLifetime, InvocationLifetime>();
        if (configureOptions is { } configureOptionsAction)
        {
            _ = host.Services.Configure(configureOptionsAction);
        }

        return host;
    }
#endif
}
#endif