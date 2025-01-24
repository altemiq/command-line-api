// -----------------------------------------------------------------------
// <copyright file="VerbosityOption.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace System.CommandLine;

/// <summary>
/// The verbosity option.
/// </summary>
public sealed class VerbosityOption : Option<VerbosityOptions>
{
    /// <summary>
    /// The name of the option.
    /// </summary>
    public const string OptionName = "--verbosity";

    /// <summary>
    /// Initialises a new instance of the <see cref="VerbosityOption"/> class.
    /// </summary>
    public VerbosityOption()
        : this(OptionName, "-v")
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="VerbosityOption"/> class.
    /// </summary>
    /// <param name="name">The name of the option. It's used for parsing, displaying Help and creating parse errors.</param>>
    /// <param name="aliases">Optional aliases. Used for parsing, suggestions and displayed in Help.</param>
    public VerbosityOption(string name, params string[] aliases)
        : base(name, aliases)
    {
        this.HelpName = "LEVEL";
        this.Description = LocalizationResources.VerbosityOptionDescription();
        this.DefaultValueFactory = _ => VerbosityOptions.normal;
        this.Recursive = true;
    }
}