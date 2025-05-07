# `System.CommandLine` extensions

## Standard Options

This package provides some common options that include Verbosity

```csharp
command.Add(new VerbosityOption());
```

## Required values

Use the `GetRequired*` methods to ensure that a value exists. These methods will throw rather than returning null.

```csharp
var value = parseResult.GetRequiredValue(option);
```

These call the base `Get*` methods, and check the value for being `null`.

## Help methods

These help with configuring the help methods, such as finding the `HelpAction`, and then configuring the output.

```csharp
command.ConfigureHelp(static builder => builder.CustomizeLayout());
```

## Invocation

### Delegate action

This sets up a wrapping action, around the current action, that will call the specified delegate before the main action.

This is useful to set an action to run before the specified action, such as configuring native PATHs, etc.

```csharp
var command = new Command("COMMAND");
command.SetAction(parseResult =>
{
    /* perform command line action */
});

Invocation.DelegateAction.SetHandlers(
    command,
    parseResult =>
    {
        /* perform sync action */
    },
    (parseResult, cancellationToken) =>
    {
        /* perform async action */
    }
```

### Instance action

This sets up a wrapping action, around the current action, that will create as instance before the main action.

This is useful to set an action to run before the specified action that an instance the relies on the parse result.

```csharp
var command = new Command("COMMAND");
command.SetAction(parseResult =>
{
    /* perform command line action */
});

Invocation.InstanceAction.SetHandlers(
    command,
    parseResult => 
    {
        /* create instance based on the parse result */
    });
```

### Builder action

This sets up a wrapping action, around the current action, that will call the specified builder before the main action.

This is useful to set an action to run before the specified action that require the builder pattern, such as logging, or hosting.

```csharp
var command = new Command("COMMAND");
command.SetAction(parseResult =>
{
    /* perform command line action */
});

Invocation.BuilderAction.SetHandlers(
    command,
    _ => hostBuilderFactory(),
    builder => builder.Build(),
    (parseResult, builder) => builder.ConfigureAppConfiguration((_, builder) => configure(parseResult, builder)));
```