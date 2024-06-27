// -----------------------------------------------------------------------
// <copyright file="ParsingHelpers.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine.Parsing;

/// <summary>
/// Parsing helpers.
/// </summary>
internal static class ParsingHelpers
{
    /// <summary>
    /// Returns a single value, or throws an exception.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="values">The source.</param>
    /// <param name="multipleException">The exception factory for when there are multiple items.</param>
    /// <param name="noneException">The exception factory for when there are no items.</param>
    /// <returns>The single item.</returns>
    public static T ReturnSingle<T>(IEnumerable<T> values, Func<Exception> multipleException, Func<Exception> noneException)
    {
        var enumerator = values.GetEnumerator();
        if (enumerator.MoveNext())
        {
            var uri = enumerator.Current;
            if (enumerator.MoveNext())
            {
                // throw exception as we have multiple
                throw multipleException();
            }

            return uri;
        }

        // throw exception as there were none.
        throw noneException();
    }
}