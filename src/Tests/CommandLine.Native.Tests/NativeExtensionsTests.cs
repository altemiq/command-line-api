namespace System.CommandLine.Native;

public class NativeExtensionsTests
{
    private static readonly string PathVariable = Altemiq.Runtime.InteropServices.RuntimeInformation.PathVariable;

    [Test]
    public async Task Test()
    {
        string? path = Environment.GetEnvironmentVariable(PathVariable);
        RootCommand rootCommand = new RootCommand().ResolveNative();
        _ = await rootCommand.Parse([]).InvokeAsync();

        _ = await Assert.That(Environment.GetEnvironmentVariable(PathVariable)).IsNotEqualTo(path);

        Environment.SetEnvironmentVariable(PathVariable, path);
    }

    [Test]
    public async Task TestTested()
    {
        string? path = Environment.GetEnvironmentVariable(PathVariable);
        RootCommand command = [];
        command.SetAction(_ => { });
        RootCommand rootCommand = new RootCommand().ResolveNative();
        _ = await rootCommand.Parse([]).InvokeAsync();

        _ = await Assert.That(Environment.GetEnvironmentVariable(PathVariable)).IsNotEqualTo(path);

        Environment.SetEnvironmentVariable(PathVariable, path);
    }

    [Test]
    public async Task TestTestedAsync()
    {
        string? path = Environment.GetEnvironmentVariable(PathVariable);
        RootCommand command = [];
        command.SetAction((_, _) => Task.CompletedTask);
        RootCommand rootCommand = new RootCommand().ResolveNative();
        _ = await rootCommand.Parse([]).InvokeAsync();

        _ = await Assert.That(Environment.GetEnvironmentVariable(PathVariable)).IsNotEqualTo(path);

        Environment.SetEnvironmentVariable(PathVariable, path);
    }
}