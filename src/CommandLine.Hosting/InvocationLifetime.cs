// -----------------------------------------------------------------------
// <copyright file="InvocationLifetime.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

#if NETSTANDARD2_0
using IHostEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;
using IHostApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;
#endif

/// <summary>
/// The invocation lifetime.
/// </summary>
public class InvocationLifetime : IHostLifetime, IDisposable
{
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
        this.Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this.Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        this.ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));

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

    private void OnInvocationCancelled() => this.ApplicationLifetime.StopApplication();

    private void OnApplicationStarted()
    {
        this.Logger.LogInformation("Application started. Press Ctrl+C to shut down.");
        this.Logger.LogInformation("Hosting environment: {envName}", this.Environment.EnvironmentName);
        this.Logger.LogInformation("Content root path: {contentRoot}", this.Environment.ContentRootPath);
    }

    private void OnApplicationStopping() => this.Logger.LogInformation("Application is shutting down...");
}