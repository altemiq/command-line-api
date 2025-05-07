// -----------------------------------------------------------------------
// <copyright file="InvocationLifetime.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

#if NETSTANDARD2_0
using IHostApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;
using IHostEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;
#endif

/// <summary>
/// The invocation lifetime.
/// </summary>
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
[Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Public API")]
public
#if NET6_0_OR_GREATER
    partial
#endif
    class InvocationLifetime : IHostLifetime, IDisposable
{
#if NETSTANDARD2_0
    private static readonly Action<ILogger, System.Exception> LogApplicationStarted = LoggerMessage.Define(LogLevel.Information, 1, "Application started. Press Ctrl+C to shut down.");
    private static readonly Action<ILogger, string, System.Exception> LogHostingEnvironment = Microsoft.Extensions.Logging.LoggerMessage.Define<string>(LogLevel.Information, 2, "Hosting environment: {EnvName}");
    private static readonly Action<ILogger, string, System.Exception> LogContentRootPath = Microsoft.Extensions.Logging.LoggerMessage.Define<string>(LogLevel.Information, 3, "Content root path: {ContentRoot}");
    private static readonly Action<ILogger, System.Exception> LogApplicationStopping = Microsoft.Extensions.Logging.LoggerMessage.Define(LogLevel.Information, 4, "Application is shutting down...");
#endif

    private CancellationTokenRegistration invokeCancelRegistration;
    private CancellationTokenRegistration appStartedRegistration;
    private CancellationTokenRegistration appStoppingRegistration;
    private bool disposedValue;

    /// <summary>
    /// Initialises a new instance of the <see cref="InvocationLifetime"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="environment">The host environment.</param>
    /// <param name="applicationLifetime">The host application lifetime.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public InvocationLifetime(
        IOptions<InvocationLifetimeOptions> options,
        IHostEnvironment environment,
        IHostApplicationLifetime applicationLifetime,
        ILoggerFactory? loggerFactory = null)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Value);
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(applicationLifetime);
        this.Options = options.Value;
        this.Environment = environment;
        this.ApplicationLifetime = applicationLifetime;
#else
        this.Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this.Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        this.ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
#endif

        this.Logger = EnsureLoggerFactory(loggerFactory).CreateLogger("Microsoft.Hosting.Lifetime");

        static ILoggerFactory EnsureLoggerFactory(ILoggerFactory? loggerFactory)
        {
            return loggerFactory ?? NullLoggerFactory.Instance;
        }
    }

    /// <summary>
    /// Gets the options.
    /// </summary>
    public InvocationLifetimeOptions Options { get; }

    /// <summary>
    /// Gets the host environment.
    /// </summary>
    public IHostEnvironment Environment { get; }

    /// <summary>
    /// Gets the host application lifetime.
    /// </summary>
    public IHostApplicationLifetime ApplicationLifetime { get; }

    private ILogger Logger { get; }

    /// <inheritdoc/>
    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        if (!this.Options.SuppressStatusMessages)
        {
            this.appStartedRegistration = this.ApplicationLifetime.ApplicationStarted.Register(state => GetInvocationLifetime(state).OnApplicationStarted(), this);
            this.appStoppingRegistration = this.ApplicationLifetime.ApplicationStopping.Register(state => GetInvocationLifetime(state).OnApplicationStopping(), this);
        }

        // The token comes from HostingAction.InvokeAsync
        // and it's the invocation cancellation token.
        this.invokeCancelRegistration = cancellationToken.Register(state => GetInvocationLifetime(state).OnInvocationCancelled(), this);

        return Task.CompletedTask;

        static InvocationLifetime GetInvocationLifetime(object? state)
        {
            return state as InvocationLifetime ?? throw new InvalidOperationException();
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    /// <param name="disposing">Set to <see langword="true"/> to disposed of managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.invokeCancelRegistration.Dispose();
                this.appStartedRegistration.Dispose();
                this.appStoppingRegistration.Dispose();
            }

            this.disposedValue = true;
        }
    }

#if NET6_0_OR_GREATER
    [LoggerMessage(1, LogLevel.Information, "Application started. Press Ctrl+C to shut down.")]
    private static partial void LogApplicationStarted(ILogger logger, Exception exception);

    [LoggerMessage(2, LogLevel.Information, "Hosting environment: {EnvName}")]
    private static partial void LogHostingEnvironment(ILogger logger, string envName, Exception exception);

    [LoggerMessage(3, LogLevel.Information, "Content root path: {ContentRoot}")]
    private static partial void LogContentRootPath(ILogger logger, string contentRoot, Exception exception);

    [LoggerMessage(4, LogLevel.Information, "Application is shutting down...")]
    private static partial void LogApplicationStopping(ILogger logger, Exception exception);
#endif

    private void OnInvocationCancelled() => this.ApplicationLifetime.StopApplication();

    private void OnApplicationStarted()
    {
        LogApplicationStarted(this.Logger, null!);
        LogHostingEnvironment(this.Logger, this.Environment.EnvironmentName, null!);
        LogContentRootPath(this.Logger, this.Environment.ContentRootPath, null!);
    }

    private void OnApplicationStopping() => LogApplicationStopping(this.Logger, null!);
}