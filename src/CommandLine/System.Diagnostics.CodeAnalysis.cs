// -----------------------------------------------------------------------
// <copyright file="System.Diagnostics.CodeAnalysis.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_0_OR_GREATER

#pragma warning disable CA1801, CA1822, SA1402, SA1600, SA1649

namespace System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
internal sealed class AllowNullAttribute : Attribute;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
internal sealed class DisallowNullAttribute : Attribute;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class DoesNotReturnAttribute : Attribute;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class DoesNotReturnIfAttribute(bool parameterValue) : Attribute
{
    public bool ParameterValue => parameterValue;
}

#if !NETSTANDARD2_0_OR_GREATER && !NET40_OR_GREATER && !NETCOREAPP2_0_OR_GREATER
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
internal sealed class ExcludeFromCodeCoverageAttribute : Attribute;
#endif

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
internal sealed class MaybeNullAttribute : Attribute;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class MaybeNullWhenAttribute(bool returnValue) : Attribute
{
    public bool ReturnValue => returnValue;
}

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
internal sealed class NotNullAttribute : Attribute;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
internal sealed class NotNullIfNotNullAttribute(string parameterName) : Attribute
{
    public string ParameterName => parameterName;
}

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class NotNullWhenAttribute(bool returnValue) : Attribute
{
    public bool ReturnValue => returnValue;
}

#endif