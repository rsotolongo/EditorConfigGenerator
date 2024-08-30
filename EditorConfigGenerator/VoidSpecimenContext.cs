//-----------------------------------------------------------------------
// <copyright file="VoidSpecimenContext.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using AutoFixture.Kernel;

namespace EditorConfigGenerator;

/// <summary>
/// Void specimen context.
/// </summary>
/// <seealso cref="ISpecimenContext" />
internal sealed class VoidSpecimenContext : ISpecimenContext
{
    /// <inheritdoc/>
    public object Resolve(object request)
    {
        return default;
    }
}
