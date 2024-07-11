// -----------------------------------------------------------------------
// <copyright file="HostedServiceTests.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class HostedServiceTests
{
    [Fact]
    public void UseHostedServiceThroughHost()
    {
        const string CommandName = "command";
        var hostedService = new HostedService();
        var configuration = new CliConfiguration(new CliRootCommand { new CliCommand(CommandName) });
        configuration.UseHost((parseResult, builder) =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddHostedService(_ => hostedService);
            });
        });

        configuration.Invoke(CommandName);

        hostedService.Started.Should().BeTrue();
        hostedService.Stopped.Should().BeTrue();
    }

#if NET7_0_OR_GREATER
    [Fact]
    public void UseHostedServiceThroughApplicationHost()
    {
        var hostedService = new HostedService();
        var configuration = new CliConfiguration(new CliRootCommand());
        configuration.UseApplicationHost((parseResult, builder) =>
        {
            builder.Services.AddHostedService(_ => hostedService);
        });

        configuration.Invoke(string.Empty);

        hostedService.Started.Should().BeTrue();
        hostedService.Stopped.Should().BeTrue();
    }
#endif

    private class HostedService : IHostedService
    {
        public bool Started { get; private set; }

        public bool Stopped { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.Started = true;
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Stopped = true;
            return Task.CompletedTask;
        }
        }
}
