// -----------------------------------------------------------------------
// <copyright file="HostingExtensions.ApplicationHost.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;

/// <content>
/// The <see cref="Microsoft.Extensions.Hosting.HostApplicationBuilder"/> extensions.
/// </content>
#if NET8_0_OR_GREATER
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
#endif
public static partial class HostingExtensions
{
    /// <summary>
    /// Use the application host builder to create a host.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="configureHost">The function to configure the host.</param>
    /// <returns>The root command for chaining.</returns>
    public static T UseApplicationHost<T>(this T rootCommand, Action<ParseResult?, Microsoft.Extensions.Hosting.HostApplicationBuilder>? configureHost = default)
        where T : RootCommand => UseApplicationHost(rootCommand, Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder, configureHost);

    /// <summary>
    /// Use the application host builder to create a host.
    /// </summary>
    /// <typeparam name="T">The type of root command.</typeparam>
    /// <param name="rootCommand">The root command.</param>
    /// <param name="hostBuilderFactory">The host builder factory.</param>
    /// <param name="configureHost">The function to configure the host.</param>
    /// <returns>The root command for chaining.</returns>
    public static T UseApplicationHost<T>(this T rootCommand, Func<string[], Microsoft.Extensions.Hosting.HostApplicationBuilder> hostBuilderFactory, Action<ParseResult?, Microsoft.Extensions.Hosting.HostApplicationBuilder>? configureHost = default)
        where T : RootCommand
    {
        rootCommand.Add(new Directive(HostingDirectiveName));

        Invocation.BuilderCommandLineAction.SetActions(
            rootCommand,
            parseResult => CreateHostApplicationBuilder(rootCommand, hostBuilderFactory, parseResult),
            static builder => builder.Build(),
            (parseResult, builder) => configureHost?.Invoke(parseResult, builder),
            static (_, host, cancellationToken) => host.StartAsync(cancellationToken),
            static (_, host, cancellationToken) => host.StopAsync(cancellationToken));

        return rootCommand;

        static Microsoft.Extensions.Hosting.HostApplicationBuilder CreateHostApplicationBuilder(T rootCommand, Func<string[], Microsoft.Extensions.Hosting.HostApplicationBuilder> hostBuilderFactory, ParseResult? parseResult)
        {
            var hostBuilder = hostBuilderFactory(GetUnmatchedTokens(parseResult));

            if (parseResult is not null)
            {
                UpdateHostConfiguration(rootCommand, parseResult, hostBuilder);
                _ = hostBuilder.Services.AddSingleton(parseResult);
            }

            _ = hostBuilder.UseInvocationLifetime();
            return hostBuilder;

            static string[] GetUnmatchedTokens(ParseResult? parseResult)
            {
                return parseResult?.UnmatchedTokens.ToArray() ?? [];
            }

            static void UpdateHostConfiguration(T rootCommand, ParseResult parseResult, Microsoft.Extensions.Hosting.HostApplicationBuilder hostBuilder)
            {
                if (rootCommand.Directives.SingleOrDefault(static d => string.Equals(d.Name, HostingDirectiveName, StringComparison.Ordinal)) is { } directive
                    && parseResult.GetResult(directive) is { } directiveResult)
                {
                    hostBuilder.Configuration.Sources.Add(
                        new MemoryConfigurationSource
                        {
                            InitialData = [.. directiveResult.Values.Select(Parse)],
                        });

                    static KeyValuePair<string, string?> Parse(string s)
                    {
                        var parts = s.Split(Separator, count: 2);
                        var key = parts[0];
                        var value = parts.Length > 1 ? parts[1] : null;
                        return new KeyValuePair<string, string?>(key, value);
                    }
                }
            }
        }
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
        _ = host.Services.AddInvocationLifetime();
        _ = host.ConfigureInvocationLifetime(configureOptions);
        return host;
    }

    /// <summary>
    /// Configures the invocation lifetime.
    /// </summary>
    /// <param name="host">The host builder.</param>
    /// <param name="configureOptions">The options to configure.</param>
    /// <returns>The host builder for chaining.</returns>
    public static Microsoft.Extensions.Hosting.IHostApplicationBuilder ConfigureInvocationLifetime(this Microsoft.Extensions.Hosting.IHostApplicationBuilder host, Action<InvocationLifetimeOptions>? configureOptions = null)
    {
        _ = host.Services.ConfigureInvocationLifetime(configureOptions);
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
        _ = host.Services.ConfigureInvocationLifetime(configureOptions);
        return host;
    }

    /// <summary>
    /// Configures the invocation lifetime.
    /// </summary>
    /// <param name="host">The host builder.</param>
    /// <param name="configureOptions">The options to configure.</param>
    /// <returns>The host builder for chaining.</returns>
    public static Microsoft.Extensions.Hosting.HostApplicationBuilder ConfigureInvocationLifetime(this Microsoft.Extensions.Hosting.HostApplicationBuilder host, Action<InvocationLifetimeOptions>? configureOptions = null)
    {
        _ = host.Services.ConfigureInvocationLifetime(configureOptions);
        return host;
    }
#endif
}