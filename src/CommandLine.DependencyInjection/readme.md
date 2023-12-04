# `System.CommandLine` extension for `Microsoft.Extensions.DependencyInjection`

This package adds support for `Microsoft.Extensions.DependencyInjection`

This adds extension methods for `UseServices` that allows configuring the services.

This is then used to get `IServiceProvider` in actions using `GetServices`

```csharp
var command = new CliCommand("COMMAND");
command.SetHandler(parseResult =>
{
    var services = parseResult.GetServices();
});

var configuration = new CliConfiguration(command);
configuration.UseServices(services =>
{
    /* configure the services */
});

configuration.Invoke(args);
```
