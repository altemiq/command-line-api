# `System.CommandLine` extension for `Microsoft.Extensions.Logging`

This package adds support for `Microsoft.Extensions.Logging`

This adds extension methods for `AddLogging` that allows configuring the logging.

This is then used to get configuration in actions using `GetConfiguration`

```csharp
var command = new CliCommand("COMMAND") { CliOptions.VerbosityOption };
command.SetHandler(parseResult =>
{
    var logger = parseResult.CreateLogger("category");
    var level = parseResult.GetLogLevel();
});

var configuration = new CliConfiguration(command);
configuration.AddLogging((parseResult, builder) =>
{
    // Add the CliConfiguration as a logging provider
    builder.AddCliConfiguration(parseResult.Configuration);

    /* configure the logging */
});

configuration.Invoke(args);
```

If the `CliOptions.VerbosityOption` is added, this sets the `MinimumLevel` on the builder.