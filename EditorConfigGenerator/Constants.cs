//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace EditorConfig
{
    /// <summary>
    /// Internal constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The assemblies pattern.
        /// </summary>
        internal const string AssembliesPattern = "*.dll";

        /// <summary>
        /// The assemblies path.
        /// </summary>
        internal const string AssembliesPath = "Assemblies";

        /// <summary>
        /// The assembly rules header pattern.
        /// </summary>
        internal const string AssemblyRulesHeaderPattern = "# Rules from assembly: {0}";

        /// <summary>
        /// The error level.
        /// </summary>
        internal const string ErrorLevel = "error";

        /// <summary>
        /// The none level.
        /// </summary>
        internal const string NoneLevel = "none";

        /// <summary>
        /// The other language pattern.
        /// </summary>
        internal const string OtherLanguagePattern = "VisualBasic";

        /// <summary>
        /// The output filename.
        /// </summary>
        internal const string OutputFilename = ".editorConfig";

        /// <summary>
        /// The package name attribute.
        /// </summary>
        internal const string PackageNameAttribute = "Include";

        /// <summary>
        /// The package node name.
        /// </summary>
        internal const string PackageNodeName = "PackageReference";

        /// <summary>
        /// The package version attribute.
        /// </summary>
        internal const string PackageVersionAttribute = "Version";

        /// <summary>
        /// The packages directory name.
        /// </summary>
        internal const string PackagesDirectoryName = ".nuget";

        /// <summary>
        /// The packages subdirectory name.
        /// </summary>
        internal const string PackagesSubdirectoryName = "packages";

        /// <summary>
        /// The project extension.
        /// </summary>
        internal const string ProjectExtension = ".csproj";

        /// <summary>
        /// The relative path for non Windows OS.
        /// </summary>
        internal const string RelativeNonWindowsPath = "../../..";

        /// <summary>
        /// The relative path for Windows OS.
        /// </summary>
        internal const string RelativeWindowsPath = "..\\..\\..";

        /// <summary>
        /// The resources pattern.
        /// </summary>
        internal const string ResourcesPattern = ".resources";

        /// <summary>
        /// The rule header pattern.
        /// </summary>
        internal const string RuleHeaderPattern = "# {0}: {1}";

        /// <summary>
        /// The rule severity pattern.
        /// </summary>
        internal const string RuleSeverityPattern = "dotnet_diagnostic.{0}.severity = {1}";

        /// <summary>
        /// The supported diagnostics property name.
        /// </summary>
        internal const string SupportedDiagnosticsPropertyName = "SupportedDiagnostics";

        /// <summary>
        /// The warning level.
        /// </summary>
        internal const string WarningLevel = "warning";

        /// <summary>
        /// The identifiers to be ìgnored.
        /// </summary>
        internal static readonly string[] NoneIds = { "CS8019", "IDE0008", "IDE0130", "S1309", "S1451", "S1694", "S4018", "S4023", "SA1101", "SA1118", "SA1200", "SX1309", "SX1309S" };

        /// <summary>
        /// The identifiers to be warned.
        /// </summary>
        internal static readonly string[] WarningIds = { "CA1056", "CA1716", "CA1724", "S1134", "S1135" };

        /// <summary>
        /// The output file header.
        /// </summary>
        internal static readonly string[] OutputFileHeader =
        {
            "# To learn more about .editorconfig see https://aka.ms/editorconfigdocs",
            string.Empty,
            "# Remove the line below if you want to inherit .editorconfig settings from higher directories",
            "root = true",
            string.Empty,
            "[*]",
            "indent_size = 2",
            "indent_style = space",
            string.Empty,
            "[*.cs]",
            "indent_size = 4",
            "dotnet_analyzer_diagnostic.severity = error",
            string.Empty,
        };
    }
}
