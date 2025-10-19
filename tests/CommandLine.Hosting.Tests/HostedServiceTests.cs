// -----------------------------------------------------------------------
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
        RootCommand rootCommand = [new Command(CommandName)];
        _ = rootCommand.UseHost((_, builder) => builder.ConfigureServices(services => services.AddHostedService(_ => hostedService)));

        _ = await rootCommand.Parse(CommandName).InvokeAsync();

        _ = await Assert.That(hostedService.Started).IsTrue();
        _ = await Assert.That(hostedService.Stopped).IsTrue();
    }

#if NET7_0_OR_GREATER
    [Test]
    public async Task UseHostedServiceThroughApplicationHost()
    {
        HostedService hostedService = new();
        RootCommand rootCommand = [];
        _ = rootCommand.UseApplicationHost((_, builder) => builder.Services.AddHostedService(_ => hostedService));

        _ = await rootCommand.Parse([]).InvokeAsync();

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