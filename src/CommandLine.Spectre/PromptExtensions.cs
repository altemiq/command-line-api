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
    /// Gets the parsed value or prompts for the value for <see cref="Option{Boolean}"/>.
    /// </summary>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="option">The option to get the value for.</param>
    /// <param name="prompt">The prompt for the user.</param>
    /// <param name="console">The console to use or <see cref="global::Spectre.Console.AnsiConsole.Console"/> if <see langword="null" />.</param>
    /// <returns>The parsed value or the prompted value for <paramref name="option"/>.</returns>
    public static bool GetValueOrPrompt(this ParseResult parseResult, Option<bool> option, string prompt, IAnsiConsole? console = default)
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

        return confirmationPrompt.Show(console.GetValueOrDefault());
    }

    /// <summary>
    /// Gets the parsed value or prompts for the value for <see cref="Option"/>.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="parseResult">The parse result.</param>
    /// <param name="option">The option to get the value for.</param>
    /// <param name="prompt">The prompt for the user.</param>
    /// <param name="console">The console to use or <see cref="global::Spectre.Console.AnsiConsole.Console"/> if <see langword="null" />.</param>
    /// <returns>The parsed value or the prompted value for <paramref name="option"/>.</returns>
    public static T GetValueOrPrompt<T>(this ParseResult parseResult, Option<T> option, string prompt, IAnsiConsole? console = default)
    {
        if (parseResult.GetResult(option) is { Implicit: false } optionResult)
        {
            return optionResult.GetValueOrDefault<T>();
        }

        return typeof(T).IsEnum
#pragma warning disable CS8714
            ? GetValueOrPromptEnum(option, prompt, console.GetValueOrDefault())
#pragma warning restore CS8714
            : GetValueOrPromptGeneric(option, prompt, console.GetValueOrDefault());

        static TEnum GetValueOrPromptEnum<TEnum>(Option<TEnum> option, string prompt, IAnsiConsole console)
            where TEnum : notnull
        {
            // do this on the enum
            return typeof(TEnum).GetCustomAttributes(inherit: false).OfType<FlagsAttribute>().Any()
                ? GetFlagValue(option, prompt, console)
                : GetValue(option, prompt, console);

            static TEnum GetFlagValue(global::System.CommandLine.Option<TEnum> option, string prompt, IAnsiConsole console)
            {
                // flags, cal select multiple
                var multiSelectionPrompt = new MultiSelectionPrompt<TEnum> { Title = prompt, };
                _ = multiSelectionPrompt.AddChoices(GetChoices(option.CompletionSources, Parse, ignoreDefault: true));
                return GetCombinedValues(typeof(TEnum).GetEnumUnderlyingType(), multiSelectionPrompt.Show(console));

                [Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Use conditional expression for return", Justification = "Checked")]
                static TEnum GetCombinedValues(Type enumUnderLyingType, IReadOnlyList<TEnum> values)
                {
                    if (enumUnderLyingType == typeof(sbyte))
                    {
                        return EnumCombiner.GetSByte(values);
                    }

                    if (enumUnderLyingType == typeof(byte))
                    {
                        return EnumCombiner.GetByte(values);
                    }

                    if (enumUnderLyingType == typeof(short))
                    {
                        return EnumCombiner.GetInt16(values);
                    }

                    if (enumUnderLyingType == typeof(ushort))
                    {
                        return EnumCombiner.GetUInt16(values);
                    }

                    if (enumUnderLyingType == typeof(int))
                    {
                        return EnumCombiner.GetInt32(values);
                    }

                    if (enumUnderLyingType == typeof(uint))
                    {
                        return EnumCombiner.GetUInt32(values);
                    }

                    if (enumUnderLyingType == typeof(long))
                    {
                        return EnumCombiner.GetInt64(values);
                    }

                    if (enumUnderLyingType == typeof(ulong))
                    {
                        return EnumCombiner.GetUInt64(values);
                    }

                    throw new NotSupportedException();
                }
            }

            static TEnum GetValue(global::System.CommandLine.Option<TEnum> option, string prompt, IAnsiConsole console)
            {
                var selectionPrompt = new SelectionPrompt<TEnum> { Title = prompt };
                _ = selectionPrompt.AddChoices(GetChoices(option.CompletionSources, Parse));
                return selectionPrompt.Show(console);
            }

            static TEnum Parse(string value)
            {
                return (TEnum)Enum.Parse(typeof(TEnum), value)!;
            }
        }

        static T GetValueOrPromptGeneric(Option<T> option, string prompt, IAnsiConsole console)
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
        var choices = completionItems.Select(static completionItem => completionItem.Label).Select(converter);
        if (ignoreDefault)
        {
            choices = choices.Where(static choice => choice is not null && !choice.Equals(default(TValue)));
        }

        return choices;
    }

    private static class EnumCombiner
    {
#if NET7_0_OR_GREATER
        public static TEnum GetSByte<TEnum>(IEnumerable<TEnum> values) => Get<sbyte, TEnum>(values);

        public static TEnum GetByte<TEnum>(IEnumerable<TEnum> values) => Get<byte, TEnum>(values);

        public static TEnum GetInt16<TEnum>(IEnumerable<TEnum> values) => Get<short, TEnum>(values);

        public static TEnum GetUInt16<TEnum>(IEnumerable<TEnum> values) => Get<ushort, TEnum>(values);

        public static TEnum GetInt32<TEnum>(IEnumerable<TEnum> values) => Get<int, TEnum>(values);

        public static TEnum GetUInt32<TEnum>(IEnumerable<TEnum> values) => Get<uint, TEnum>(values);

        public static TEnum GetInt64<TEnum>(IEnumerable<TEnum> values) => Get<long, TEnum>(values);

        public static TEnum GetUInt64<TEnum>(IEnumerable<TEnum> values) => Get<ulong, TEnum>(values);

        private static TEnum Get<T, TEnum>(IEnumerable<TEnum> values)
            where T : struct, Numerics.IBitwiseOperators<T, T, T>
        {
            T value = default;
            foreach (var v in values.Cast<T>())
            {
                value |= v;
            }

            return (TEnum)(object)value;
        }
#else
        public static TEnum GetSByte<TEnum>(IEnumerable<TEnum> values) => Get<sbyte, TEnum>(values, static (value, v) => (sbyte)(value | v));

        public static TEnum GetByte<TEnum>(IEnumerable<TEnum> values) => Get<byte, TEnum>(values, static (value, v) => (byte)(value | v));

        public static TEnum GetInt16<TEnum>(IEnumerable<TEnum> values) => Get<short, TEnum>(values, static (value, v) => (short)(value | v));

        public static TEnum GetUInt16<TEnum>(IEnumerable<TEnum> values) => Get<ushort, TEnum>(values, static (value, v) => (ushort)(value | v));

        public static TEnum GetInt32<TEnum>(IEnumerable<TEnum> values) => Get<int, TEnum>(values, static (value, v) => value | v);

        public static TEnum GetUInt32<TEnum>(IEnumerable<TEnum> values) => Get<uint, TEnum>(values, static (value, v) => value | v);

        public static TEnum GetInt64<TEnum>(IEnumerable<TEnum> values) => Get<long, TEnum>(values, static (value, v) => value | v);

        public static TEnum GetUInt64<TEnum>(IEnumerable<TEnum> values) => Get<ulong, TEnum>(values, static (value, v) => value | v);

        private static TEnum Get<T, TEnum>(IEnumerable<TEnum> values, Func<T, T, T> orFunc)
           where T : struct
        {
            T value = default;
            foreach (var v in values.Cast<T>())
            {
                value = orFunc(value, v);
            }

            return (TEnum)(object)value;
        }
#endif
    }
}