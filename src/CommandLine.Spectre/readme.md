# `System.CommandLine` extensions for `Spectre.Console`

This package adds support for `Spectre.Console`

## Prompting

This allows prompting for a value when it is not provided on the command line.

This has support for `Enum` (with or without `[Flags]`), primitve types, and anything that has a converter from `string`.

```csharp
var value = parseResult.GetValueOrPrompt(option, "Enter/Select the value");
```

This respects default values, and completion sources for the option.

## FigletText

This has helper methods for adding figlet text to the help action.

```csharp
var command = new Command("COMMAND");

var rootCommand = new RootCommand { command };

command.AddFiglet("figlet", Colors.Blue);
```

These need to be called after adding the command to the root command, otherwise the help action is not available.