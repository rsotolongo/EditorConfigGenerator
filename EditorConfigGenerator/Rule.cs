﻿//-----------------------------------------------------------------------
// <copyright file="Rule.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace EditorConfigGenerator;

/// <summary>
/// Rule entity.
/// </summary>
internal sealed class Rule
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    internal string Id { get; init; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    internal string Title { get; init; }
}
