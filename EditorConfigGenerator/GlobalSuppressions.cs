//-----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

//// This file is used by Code Analysis to maintain SuppressMessage
//// attributes that are applied to this project.
//// Project-level suppressions either have no target or are given
//// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Maintainability",
    "CA1508:Avoid dead conditional code",
    Justification = "RS",
    Scope = "member",
    Target = "~M:EditorConfig.Helpers.FindTypeRules(System.Type,System.Reflection.PropertyInfo)~System.Collections.Generic.List{EditorConfig.Rule}")]
[assembly: SuppressMessage(
    "Major Code Smell",
    "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields",
    Justification = "RS",
    Scope = "member",
    Target = "~M:EditorConfig.Helpers.CreateType(System.Type)~System.Object")]
[assembly: SuppressMessage(
    "Major Code Smell",
    "S3885:\"Assembly.Load\" should be used",
    Justification = "RS",
    Scope = "member",
    Target = "~M:EditorConfig.Parser.AddAssembly(System.Collections.Generic.List{System.Reflection.Assembly},System.String,System.String,System.Reflection.Assembly)")]
