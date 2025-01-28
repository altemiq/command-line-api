namespace System.CommandLine.Native;

public class NativeExtensionsTests
{
    private static readonly string PathVariable = Altavec.Runtime.InteropServices.RuntimeInformation.PathVariable;

    [Test]
    public async Task Test()
    {
        string? path = Environment.GetEnvironmentVariable(PathVariable);
        _ = new CommandLineConfiguration(new RootCommand())
            .ResolveNative()
            .Invoke(string.Empty);

        string? newPath = Environment.GetEnvironmentVariable(PathVariable);
        _ = await Assert.That(newPath).IsNotEqualTo(path);
    }

    [Test]
    public async Task TestTested()
    {
        string? path = Environment.GetEnvironmentVariable(PathVariable);
        RootCommand command = [];
        command.SetAction(_ => { });
        _ = new CommandLineConfiguration(new RootCommand())
            .ResolveNative()
            .Invoke(string.Empty);

        string? newPath = Environment.GetEnvironmentVariable(PathVariable);
        _ = await Assert.That(newPath).IsNotEqualTo(path);
    }

    [Test]
    public async Task TestTestedAsync()
    {
        string? path = Environment.GetEnvironmentVariable(PathVariable);
        RootCommand command = [];
        command.SetAction((_, _) => Task.CompletedTask);
        _ = await new CommandLineConfiguration(new RootCommand())
            .ResolveNative()
            .InvokeAsync(string.Empty);

        string? newPath = Environment.GetEnvironmentVariable(PathVariable);
        _ = await Assert.That(newPath).IsNotEqualTo(path);
    }
}
