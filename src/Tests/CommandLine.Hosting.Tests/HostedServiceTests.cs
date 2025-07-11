﻿// -----------------------------------------------------------------------
// <copyright file="HostedServiceTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class HostedServiceTests
{
    [Test]
    public async Task UseHostedServiceThroughHost()
    {
        const string CommandName = "command";
        HostedService hostedService = new();
        CommandLineConfiguration configuration = new(new RootCommand { new Command(CommandName) });
        _ = configuration.UseHost((_, builder) => builder.ConfigureServices(services => services.AddHostedService(_ => hostedService)));

        _ = await configuration.InvokeAsync(CommandName);

        _ = await Assert.That(hostedService.Started).IsTrue();
        _ = await Assert.That(hostedService.Stopped).IsTrue();
    }

#if NET7_0_OR_GREATER
    [Test]
    public async Task UseHostedServiceThroughApplicationHost()
    {
        HostedService hostedService = new();
        CommandLineConfiguration configuration = new(new RootCommand());
        _ = configuration.UseApplicationHost((_, builder) => builder.Services.AddHostedService(_ => hostedService));

        _ = await configuration.InvokeAsync(string.Empty);

        _ = await Assert.That(hostedService.Started).IsTrue();
        _ = await Assert.That(hostedService.Stopped).IsTrue();
    }
#endif

    private class HostedService : IHostedService
    {
        public bool Started { get; private set; }

        public bool Stopped { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Started = true;
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Stopped = true;
            return Task.CompletedTask;
        }
    }
}