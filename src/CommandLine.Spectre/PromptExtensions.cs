// -----------------------------------------------------------------------
// <copyright file="PromptExtensions.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The <see cref="global::Spectre.Console.AnsiConsole.Prompt{T}(IPrompt{T})"/> <see cref="ParseResult"/> extensions.
/// </summary>
public static class PromptExtensions
{
    /// <summary>
    /// Gets the parsed value or prompts for the value for <see cref="CliOption{Boolean}"/>.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="option">The option to get the value for.</param>
    /// <param name="prompt">The prompt for the user.</param>
    /// <param name="console">The console to use or <see cref="global::Spectre.Console.AnsiConsole.Console"/> if <see langword="null" />.</param>
    /// <returns>The parsed value or the prompted value for <paramref name="option"/>.</returns>
    public static bool GetValueOrPrompt(this ParseResult parseResult, CliOption<bool> option, string prompt, IAnsiConsole? console = default)
    {
        if (parseResult.GetResult(option) is { Implicit: false } optionResult)
        {
            return optionResult.GetValueOrDefault<bool>();
        }

        var confirmationPrompt = new ConfirmationPrompt(prompt);
        if (option.HasDefaultValue && option.DefaultValueFactory is { } defaultValueFactory)
        {
            _ = confirmationPrompt.ShowDefaultValue();
            confirmationPrompt.DefaultValue = defaultValueFactory(default!)!;
        }

        return confirmationPrompt.Show(AnsiConsole.GetConsoleOrDefault(console));
    }

    /// <summary>
    /// Gets the parsed value or prompts for the value for <see cref="CliOption"/>.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="option">The option to get the value for.</param>
    /// <param name="prompt">The prompt for the user.</param>
    /// <param name="console">The console to use or <see cref="global::Spectre.Console.AnsiConsole.Console"/> if <see langword="null" />.</param>
    /// <returns>The parsed value or the prompted value for <paramref name="option"/>.</returns>
    public static T GetValueOrPrompt<T>(this ParseResult parseResult, CliOption<T> option, string prompt, IAnsiConsole? console = default)
    {
        if (parseResult.GetResult(option) is { Implicit: false } optionResult)
        {
            return optionResult.GetValueOrDefault<T>();
        }

        return typeof(T).IsEnum
#pragma warning disable CS8714
            ? GetValueOrPromptEnum(option, prompt, AnsiConsole.GetConsoleOrDefault(console))
#pragma warning restore CS8714
            : GetValueOrPromptGeneric(option, prompt, AnsiConsole.GetConsoleOrDefault(console));

        static TEnum GetValueOrPromptEnum<TEnum>(CliOption<TEnum> option, string prompt, IAnsiConsole console)
            where TEnum : notnull
        {
            // do this on the enum
            if (typeof(TEnum).GetCustomAttributes(inherit: false).OfType<FlagsAttribute>().Any())
            {
                // flags, cal select multiple
                var multiSelectionPrompt = new MultiSelectionPrompt<TEnum> { Title = prompt, };
                _ = multiSelectionPrompt.AddChoices(GetChoices(option.CompletionSources, Parse, ignoreDefault: true));
                var values = multiSelectionPrompt.Show(console);

                return typeof(TEnum).GetEnumUnderlyingType() switch
                {
                    var t when t == typeof(sbyte) => (TEnum)EnumCombiner.GetSByte(values),
                    var t when t == typeof(byte) => (TEnum)EnumCombiner.GetByte(values),
                    var t when t == typeof(short) => (TEnum)EnumCombiner.GetInt16(values),
                    var t when t == typeof(ushort) => (TEnum)EnumCombiner.GetUInt16(values),
                    var t when t == typeof(int) => (TEnum)EnumCombiner.GetInt32(values),
                    var t when t == typeof(uint) => (TEnum)EnumCombiner.GetUInt32(values),
                    var t when t == typeof(long) => (TEnum)EnumCombiner.GetInt64(values),
                    var t when t == typeof(ulong) => (TEnum)EnumCombiner.GetUInt64(values),
                    _ => throw new NotSupportedException(),
                };
            }

            var selectionPrompt = new SelectionPrompt<TEnum> { Title = prompt };
            _ = selectionPrompt.AddChoices(GetChoices(option.CompletionSources, Parse));
            return selectionPrompt.Show(console);

            static TEnum Parse(string value)
            {
                return (TEnum)Enum.Parse(typeof(TEnum), value)!;
            }
        }

        static T GetValueOrPromptGeneric(CliOption<T> option, string prompt, IAnsiConsole console)
        {
            var textPrompt = new TextPrompt<T>(prompt) { ShowDefaultValue = false, ShowChoices = false };

            if (option.HasDefaultValue && option.DefaultValueFactory is { } defaultValueFactory)
            {
                _ = textPrompt
                    .ShowDefaultValue()
                    .DefaultValue(defaultValueFactory(default!));
            }

            if (option.CompletionSources is { Count: > 0 } completionSources)
            {
                _ = textPrompt
                    .ShowChoices()
                    .AddChoices(GetChoices<T>(completionSources));
            }

            return textPrompt.Show(console);
        }
    }

    private static IEnumerable<T> GetChoices<T>(IEnumerable<Func<Completions.CompletionContext, IEnumerable<Completions.CompletionItem>>> completionSources)
    {
        var converter = GetTypeConverter();
        return GetChoices(completionSources, label => (T)converter.ConvertFromString(label)!);

        static ComponentModel.TypeConverter GetTypeConverter()
        {
            return ComponentModel.TypeDescriptor.GetConverter(typeof(T))
                ?? GetTypeConverterCore()
                ?? throw new InvalidOperationException(string.Format(Spectre.Properties.Resources.Culture, Spectre.Properties.Resources.TypeConverterNotFound, typeof(T)));

            static ComponentModel.TypeConverter? GetTypeConverterCore()
            {
                return GetConverterType() is { } type
                    ? Activator.CreateInstance(type) as ComponentModel.TypeConverter
                    : default;

                static Type? GetConverterType()
                {
                    return typeof(T).GetCustomAttributes(inherit: false).OfType<ComponentModel.TypeConverterAttribute>().FirstOrDefault() is { } customAttribute
                        ? Type.GetType(customAttribute.ConverterTypeName, throwOnError: false, ignoreCase: false)
                        : default;
                }
            }
        }
    }

    private static IEnumerable<TValue> GetChoices<TValue>(IEnumerable<Func<Completions.CompletionContext, IEnumerable<Completions.CompletionItem>>> completionSources, Func<string, TValue> converter, bool ignoreDefault = true) => GetChoices(completionSources.SelectMany(func => func(default!)), converter, ignoreDefault);

    private static IEnumerable<TValue> GetChoices<TValue>(IEnumerable<Completions.CompletionItem> completionItems, Func<string, TValue> converter, bool ignoreDefault = true)
    {
        var choices = completionItems.Select(completionItem => completionItem.Label).Select(converter);
        if (ignoreDefault)
        {
            choices = choices.Where(choice => choice is not null && !choice.Equals(default(TValue)));
        }

        return choices;
    }

    private static class EnumCombiner
    {
#if NET7_0_OR_GREATER
        public static object GetSByte<TEnum>(IEnumerable<TEnum> values) => Get<sbyte, TEnum>(values);

        public static object GetByte<TEnum>(IEnumerable<TEnum> values) => Get<byte, TEnum>(values);

        public static object GetInt16<TEnum>(IEnumerable<TEnum> values) => Get<short, TEnum>(values);

        public static object GetUInt16<TEnum>(IEnumerable<TEnum> values) => Get<ushort, TEnum>(values);

        public static object GetInt32<TEnum>(IEnumerable<TEnum> values) => Get<int, TEnum>(values);

        public static object GetUInt32<TEnum>(IEnumerable<TEnum> values) => Get<uint, TEnum>(values);

        public static object GetInt64<TEnum>(IEnumerable<TEnum> values) => Get<long, TEnum>(values);

        public static object GetUInt64<TEnum>(IEnumerable<TEnum> values) => Get<ulong, TEnum>(values);

        private static object Get<T, TEnum>(IEnumerable<TEnum> values)
            where T : struct, Numerics.IBitwiseOperators<T, T, T>
        {
            T value = default;
            foreach (var v in values.Cast<T>())
            {
                value |= v;
            }

            return value;
        }
#else
        public static object GetSByte<TEnum>(IEnumerable<TEnum> values)
        {
            sbyte value = default;
            foreach (var v in values.Cast<sbyte>())
            {
                value |= v;
            }

            return value;
        }

        public static object GetByte<TEnum>(IEnumerable<TEnum> values)
        {
            byte value = default;
            foreach (var v in values.Cast<byte>())
            {
                value |= v;
            }

            return value;
        }

        public static object GetInt16<TEnum>(IEnumerable<TEnum> values)
        {
            short value = default;
            foreach (var v in values.Cast<short>())
            {
                value |= v;
            }

            return value;
        }

        public static object GetUInt16<TEnum>(IEnumerable<TEnum> values)
        {
            ushort value = default;
            foreach (var v in values.Cast<ushort>())
            {
                value |= v;
            }

            return value;
        }

        public static object GetInt32<TEnum>(IEnumerable<TEnum> values)
        {
            int value = default;
            foreach (var v in values.Cast<int>())
            {
                value |= v;
            }

            return value;
        }

        public static object GetUInt32<TEnum>(IEnumerable<TEnum> values)
        {
            uint value = default;
            foreach (var v in values.Cast<uint>())
            {
                value |= v;
            }

            return value;
        }

        public static object GetInt64<TEnum>(IEnumerable<TEnum> values)
        {
            long value = default;
            foreach (var v in values.Cast<long>())
            {
                value |= v;
            }

            return value;
        }

        public static object GetUInt64<TEnum>(IEnumerable<TEnum> values)
        {
            ulong value = default;
            foreach (var v in values.Cast<ulong>())
            {
                value |= v;
            }

            return value;
        }
#endif
    }
}