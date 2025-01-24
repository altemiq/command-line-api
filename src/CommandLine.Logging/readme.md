# `System.CommandLine` extension for `Microsoft.Extensions.Logging`

This package adds support for `Microsoft.Extensions.Logging`

This adds extension methods for `AddLogging` that allows configuring the logging.

This is then used to get configuration in actions using `GetConfiguration`

```csharp
var command = new Command("COMMAND") { new VerbosityOption() };
command.SetHandler(parseResult =>
{
    var logger = parseResult.CreateLogger("category");
    var level = parseResult.GetLogLevel();
});

var configuration = new CommandLineConfiguration(command);
configuration.AddLogging((parseResult, builder) =>
{
    // Add the CommandLineConfiguration as a logging provider
    builder.AddCommandLineConfiguration(parseResult.Configuration);

    /* configure the logging */
});

configuration.Invoke(args);
```

If a `VerbosityOption` is added, this sets the `MinimumLevel` on the builder.