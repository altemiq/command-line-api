// -----------------------------------------------------------------------
// <copyright file="CommandExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Invocation;

internal static class CommandExtensions
{
    public static T SetAction<T>(this T command, Action action)
        where T : Command
    {
        if (action == Action.Synchronous)
        {
            command.SetAction(_ => { });
        }
        else if (action == Action.Asynchronous)
        {
            command.SetAction((_, _) => Task.CompletedTask);
        }

        return command;
    }
}