# `System.CommandLine` extension for `Microsoft.Extensions.Configuration`

This package adds support for `Microsoft.Extensions.Configuration.IConfiguration`

This adds extension methods for `UseConfiguration` that allows configuring the configuration.

This is then used to get configuration in actions using `GetConfiguration`

```csharp
var command = new CliCommand("COMMAND");
command.SetHandler(parseResult =>
{
    var configuration = parseResult.GetConfiguration();
});

var configuration = new CliConfiguration(command);
configuration.UseConfiguration(builder =>
{
    /* configure the builder */
});

configuration.Invoke(args);
```
