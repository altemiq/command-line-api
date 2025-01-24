# `System.CommandLine` extensions for file globbing

This has methods to be used as the custom parser to allow for full file system globbing.

```csharp
var argument = new Argument<FileInfo[]>("FILES") { CustomParser = CommandLine.Parsing.FileSystemGlobbingParser.Parse };
```