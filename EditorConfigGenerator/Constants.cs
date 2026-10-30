//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Text;

namespace EditorConfigGenerator;

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
    internal const string OutputFilename = ".editorconfig";

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
    /// The section separator comment.
    /// </summary>
    internal const string SectionSeparator = "# ---------------------------------------------------------------------------";

    /// <summary>
    /// The supported diagnostics property name.
    /// </summary>
    internal const string SupportedDiagnosticsPropertyName = "SupportedDiagnostics";

    /// <summary>
    /// The warning level.
    /// </summary>
    internal const string WarningLevel = "warning";

    /// <summary>
    /// The assembly rules header pattern.
    /// </summary>
    internal static readonly CompositeFormat AssemblyRulesHeaderPattern = CompositeFormat.Parse("# Rules from assembly: {0}");

    /// <summary>
    /// The deprecated title patterns.
    /// </summary>
    internal static readonly string[] DeprecatedTitlePatterns =
    [
        "[deprecated",
        "(deprecated",
    ];

    /// <summary>
    /// The identifiers to be ìgnored.
    /// </summary>
    internal static readonly string[] NoneIds =
    [
        "RCS1002FadeOut",
        "RCS1004FadeOut",
        "RCS1005FadeOut",
        "RCS1006FadeOut",
        "RCS1015FadeOut",
        "RCS1021FadeOut",
        "RCS1031FadeOut",
        "RCS1032FadeOut",
        "RCS1047FadeOut",
        "RCS1048FadeOut",
        "RCS1049FadeOut",
        "RCS1058FadeOut",
        "RCS1061FadeOut",
        "RCS1073FadeOut",
        "RCS1085FadeOut",
        "RCS1089FadeOut",
        "RCS1112FadeOut",
        "RCS1114FadeOut",
        "RCS1124FadeOut",
        "RCS1174FadeOut",
        "RCS1212FadeOut",
        "RCS1214FadeOut",
        "RCS1217FadeOut",
        "S9999-cpd",
        "S9999-log",
        "S9999-metadata",
        "S9999-metrics",
        "S9999-symbolRef",
        "S9999-telemetry",
        "S9999-testMethodDeclaration",
        "S9999-token-type",
        "S9999-warning",
        "SCS0000",
        "Debug",
        "IDE1100",
        "IDE0005_gen",
        "RemoveUnnecessaryImportsFixable",
        "Roslynator.FindSymbols.FindSymbolAnalyzer",
        "ROS0003",
        "SA1101",
        "SX1101",
        "IDE0009",
        "RCS1002",
        "RCS1004",
        "RCS1217",
        "MA0177",
        "RCS1181",
        "MA0157",
        "CA2216",
        "S4002",
        "S3971",
        "S3234",
        "S1155",
        "CA1062",
        "IDE0045",
        "IDE0046",
        "S131",
        "S3874",
        "CA1021",
        "CA1045",
        "S2339",
        "CS9208",
        "CS9209",
        "CA1812",
        "MA0182",
        "S3242",
        "CS3000",
        "CS3001",
        "CS3002",
        "CS3003",
        "CS3005",
        "CS3006",
        "CS3007",
        "CS3008",
        "CS3009",
        "CS3010",
        "CS3011",
        "CS3012",
        "CS3013",
        "CS3014",
        "CS3015",
        "CS3016",
        "CS3017",
        "CS3018",
        "CS3019",
        "CS3021",
        "CS3022",
        "CS3023",
        "CS3024",
        "CS3026",
        "CS3027",
        "CS3028",
        "CA1014",
        "S3990",
        "CA1017",
        "S3992",
        "CS8002",
        "CA2153",
        "CA2235",
        "CA2237",
        "S3925",
        "S3926",
        "S3927",
        "S4212",
        "SCS0017",
        "SCS0021",
        "SCS0022",
        "SCS0023",
        "SCS0024",
        "SCS0030",
        "SCS0032",
        "SCS0033",
        "SCS0034",
        "CA5363",
        "CA5365",
        "CA5368",
        "S5753",
        "CA1303",
        "S4055",
        "CA1040",
        "S4023",
        "CA1034",
        "CA1819",
        "CA1024",
        "CA1030",
        "CA1033",
        "CA1058",
        "CA1000",
        "MA0018",
        "S2743",
        "RCS1158",
        "CA2227",
        "CA1063",
        "S3881",
        "S2360",
        "S1312",
        "S4017",
        "S107",
        "S6960",
        "S6967",
        "S6968",
        "CA1509",
        "IDE0073",
        "S1451",
        "SA1633",
        "SA1634",
        "SA1635",
        "SA1636",
        "SA1637",
        "SA1638",
        "SA1639",
        "SA1640",
        "SA1641",
        "SA1412",
        "CS8019",
        "S1128",
        "S1144",
        "CA1823",
        "S1172",
        "RCS1163",
        "S1481",
        "MA0038",
        "MA0041",
        "S2325",
        "S1135",
        "MA0026",
        "SA1309",
        "SX1309S",
        "IDE0160",
        "SA1200",
        "SA1135",
        "RCS1264",
        "MA0004",
        "S3216",
        "RCS1090",
        "SA1000",
        "SA1001",
        "SA1002",
        "SA1003",
        "SA1004",
        "SA1005",
        "SA1006",
        "SA1007",
        "SA1008",
        "SA1009",
        "SA1010",
        "SA1011",
        "SA1012",
        "SA1013",
        "SA1014",
        "SA1015",
        "SA1016",
        "SA1017",
        "SA1018",
        "SA1019",
        "SA1020",
        "SA1021",
        "SA1022",
        "SA1023",
        "SA1024",
        "SA1025",
        "SA1026",
        "SA1027",
        "SA1028",
        "SA1500",
        "SA1501",
        "SA1502",
        "SA1504",
        "SA1505",
        "SA1506",
        "SA1507",
        "SA1508",
        "SA1509",
        "SA1510",
        "SA1511",
        "SA1512",
        "SA1513",
        "SA1514",
        "SA1515",
        "SA1516",
        "SA1517",
        "SA1518",
        "SA1600",
        "SA1601",
        "SA1602",
        "SA1604",
        "SA1605",
        "SA1606",
        "SA1607",
        "SA1608",
        "SA1609",
        "SA1610",
        "SA1611",
        "SA1615",
        "SA1616",
        "SA1617",
        "SA1618",
        "SA1619",
        "SA1623",
        "SA1624",
        "SA1625",
        "SA1626",
        "SA1627",
        "SA1628",
        "SA1629",
        "SA1630",
        "SA1631",
        "SA1632",
        "SA1642",
        "SA1643",
        "SA1644",
        "SA1645",
        "SA1646",
        "SA1647",
        "SA1648",
        "SA1649",
        "SA1650",
        "SA1651",
        "RCS1138",
        "RCS1139",
        "RCS1140",
        "RCS1141",
        "RCS1142",
        "MA0197",
        "CA1502",
        "CA1505",
        "CA1506",
        "S1541",
        "S138",
        "S104",
        "S1200",
        "CA1710",
        "CA1711",
        "CA1716",
        "CA1720",
        "CA1724",
        "S100",
        "S101",
        "SA1300",
        "SA1302",
        "SA1303",
        "SA1304",
        "SA1305",
        "SA1306",
        "SA1307",
        "SA1308",
        "SA1310",
        "SA1312",
        "SA1313",
        "CA2000",
        "IDE0058",
        "S109",
        "MA0006",
        "MA0127",
        "MA0169",
        "RCS1016",
        "IDE0047",
        "SA1119",
        "SA1119_p",
        "IDE0211",
        "SA1118",
        "RCS1213",
    ];

    /// <summary>
    /// The rule header pattern.
    /// </summary>
    internal static readonly CompositeFormat RuleHeaderPattern = CompositeFormat.Parse("# {0}: {1}");

    /// <summary>
    /// The rule severity pattern.
    /// </summary>
    internal static readonly CompositeFormat RuleSeverityPattern = CompositeFormat.Parse("dotnet_diagnostic.{0}.severity = {1}");

    /// <summary>
    /// The identifiers to be warned.
    /// </summary>
    internal static readonly string[] WarningIds =
    [
    ];

    /// <summary>
    /// The output file footer.
    /// </summary>
    internal static readonly string[] OutputFileFooter =
    [
        SectionSeparator,
        "# Generated code (obj/bin, source generators): CS* compiler diagnostics still fire here",
        "# This section must stay last: later sections override earlier ones",
        SectionSeparator,
        "[{**/obj/**/*.cs,**/bin/**/*.cs,*.g.cs,*.generated.cs,*.Designer.cs}]",
        "generated_code = true",
        "dotnet_diagnostic.CS8933.severity = none",
        "dotnet_diagnostic.CS1591.severity = none",
    ];

    /// <summary>
    /// The output file header.
    /// </summary>
    internal static readonly string[] OutputFileHeader =
    [
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
        "dotnet_analyzer_diagnostic.severity = error",
        "indent_size = 4",
        "insert_final_newline = true",
        "trim_trailing_whitespace = true",
        string.Empty,
        SectionSeparator,
        "# Style options (target: .NET 10 / C# 14)",
        SectionSeparator,
        "csharp_style_namespace_declarations = file_scoped:error",
        "csharp_using_directive_placement = outside_namespace:error",
        "csharp_style_prefer_top_level_statements = true:error",
        string.Empty,
        "# var only when the type is apparent",
        "csharp_style_var_for_built_in_types = false:error",
        "csharp_style_var_when_type_is_apparent = true:error",
        "csharp_style_var_elsewhere = false:error",
        string.Empty,
        "# Expression bodies: simple members yes, methods/constructors/local functions no",
        "csharp_style_expression_bodied_properties = true:error",
        "csharp_style_expression_bodied_indexers = true:error",
        "csharp_style_expression_bodied_accessors = true:error",
        "csharp_style_expression_bodied_lambdas = true:error",
        "csharp_style_expression_bodied_operators = true:error",
        "csharp_style_expression_bodied_methods = false:error",
        "csharp_style_expression_bodied_constructors = false:error",
        "csharp_style_expression_bodied_local_functions = false:error",
        string.Empty,
        "# Formatting (enforced by IDE0055 / dotnet format)",
        "csharp_new_line_before_open_brace = all",
        "csharp_new_line_before_else = true",
        "csharp_new_line_before_catch = true",
        "csharp_new_line_before_finally = true",
        "csharp_space_after_keywords_in_control_flow_statements = true",
        "csharp_space_between_method_call_parameter_list_parentheses = false",
        "csharp_indent_case_contents = true",
        "csharp_preserve_single_line_statements = false",
        string.Empty,
        "# Roslynator / Sonar / Meziantou options",
        "roslynator_empty_string_style = field",
        "roslynator_doc_comment_summary_style = multi_line",
        "dotnet_code_quality.S3776.threshold = 15",
        "dotnet_code_quality.S134.max = 4",
        "MA0051.maximum_lines_per_method = 60",
        "MA0051.maximum_statements_per_method = 40",
        string.Empty,
        SectionSeparator,
        "# Naming rules (static readonly first: first match wins)",
        SectionSeparator,
        "dotnet_naming_rule.static_readonly_pascal.symbols = static_readonly_fields",
        "dotnet_naming_rule.static_readonly_pascal.style = pascal",
        "dotnet_naming_rule.static_readonly_pascal.severity = error",
        "dotnet_naming_symbols.static_readonly_fields.applicable_kinds = field",
        "dotnet_naming_symbols.static_readonly_fields.required_modifiers = static, readonly",
        string.Empty,
        "dotnet_naming_rule.private_instance_fields_underscore.symbols = private_fields",
        "dotnet_naming_rule.private_instance_fields_underscore.style = underscore_camel",
        "dotnet_naming_rule.private_instance_fields_underscore.severity = error",
        "dotnet_naming_symbols.private_fields.applicable_kinds = field",
        "dotnet_naming_symbols.private_fields.applicable_accessibilities = private",
        string.Empty,
        "dotnet_naming_style.pascal.capitalization = pascal_case",
        "dotnet_naming_style.underscore_camel.required_prefix = _",
        "dotnet_naming_style.underscore_camel.capitalization = camel_case",
        string.Empty,
    ];
}
