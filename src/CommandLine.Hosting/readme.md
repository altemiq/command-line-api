# `System.CommandLine` extension for `Microsoft.Extensions.Hosting`

This package adds support for `Microsoft.Extensions.Hosting`, for both `Configuration` and `DependencyInjection`.

This adds extension methods for `UseServices` that allows configuring the services, and `UseConfiguration` that allows configuring the configuration.

This is then used to get `IServiceProvider` in actions using `GetServices`, and `GetConfiguration` for `IConfiguration`.

```csharp
var command = new CliCommand("COMMAND");
command.SetHandler(parseResult =>
{
    var services = parseResult.GetServices();
    var configuration = parseResult.GetConfiguration();
});

var configuration = new CliConfiguration(command);
configuration
    .UseServices(services =>
    {
        /* configure the services */
    })
    .UseConfiguration(builder =>
    {
        /* configure the builder */
    });

configuration.Invoke(args);
```
