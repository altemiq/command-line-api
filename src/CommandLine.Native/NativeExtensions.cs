// -----------------------------------------------------------------------
// <copyright file="NativeExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The native extensions.
/// </summary>
[Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class NativeExtensions
{
    /// <summary>
    /// Resolves native assemblies for the configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration.</typeparam>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The input configuration.</returns>
    public static T ResolveNative<T>(this T configuration)
        where T : CommandLineConfiguration
    {
        NativeAction.SetHandlers(configuration.RootCommand);
        return configuration;
    }

    private static class NativeAction
    {
        public static void SetHandlers(Command command)
        {
            command.Action = command.Action switch
            {
                Invocation.AsynchronousCommandLineAction asyncAction => new NativeNestedAsynchronousAction(asyncAction),
                Invocation.SynchronousCommandLineAction syncAction => new NativeNestedSynchronousAction(syncAction),
                null => new NativeSynchronousAction(),
                var a => a,
            };

            foreach (var subCommand in command.Subcommands)
            {
                SetHandlers(subCommand);
            }
        }

        private static void BeforeInvoke()
        {
            if (Altemiq.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeNativeDirectory() is { } nativeDirectory)
            {
                Altemiq.Runtime.InteropServices.RuntimeEnvironment.AddDirectoryToPath(nativeDirectory);
            }

            Altemiq.Runtime.Resolve.RuntimeAssemblies();
        }

        private static void AfterInvoke() => Altemiq.Runtime.Resolve.Remove();

        private sealed class NativeNestedAsynchronousAction(Invocation.AsynchronousCommandLineAction actualAction) : Invocation.NestedAsynchronousCommandLineAction(actualAction, (_, cancellationToken) => Task.Run(BeforeInvoke, cancellationToken), (_, cancellationToken) => Task.Run(AfterInvoke, cancellationToken));

        private sealed class NativeNestedSynchronousAction(Invocation.SynchronousCommandLineAction actualAction) : Invocation.NestedSynchronousCommandLineAction(actualAction, _ => BeforeInvoke(), _ => AfterInvoke());

        private sealed class NativeSynchronousAction : Invocation.SynchronousCommandLineAction
        {
            public override int Invoke(ParseResult parseResult)
            {
                BeforeInvoke();
                AfterInvoke();
                return 0;
            }
        }
    }
}