//-----------------------------------------------------------------------
// <copyright file="Parser.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;

namespace EditorConfigGenerator;

/// <summary>
/// Parser implementation.
/// </summary>
internal sealed class Parser
{
    private readonly Assembly[] _assemblies;

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    internal Parser()
        : this(Helpers.GetFiles())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="assemblyPaths">The assembly paths.</param>
    internal Parser(string[] assemblyPaths)
    {
        var assembliesList = new List<Assembly>();
        if (assemblyPaths is not null)
        {
            foreach (string path in assemblyPaths.Where(File.Exists))
            {
                var assemblyName = AssemblyName.GetAssemblyName(path);
                string fullName = assemblyName?.FullName;
                string name = assemblyName?.Name;
                Assembly referenceAssembly = assembliesList.Find(item => string.Equals(item.FullName, fullName, StringComparison.Ordinal));
                assembliesList.AddRange(LoadAssembly(path, name, referenceAssembly));
            }
        }

        _assemblies = [.. assembliesList];
    }

    /// <summary>
    /// Gets the assemblies rule severeties.
    /// </summary>
    /// <param name="noneIds">The rules identifiers to ignore.</param>
    /// <param name="warningIds">The rules identifiers to warn about.</param>
    /// <param name="addHeader">If set to <see langword="true"/> [add header].</param>
    /// <param name="addSeparator">If set to <see langword="true"/> [add separator].</param>
    /// <param name="attendDeprecated">If set to <see langword="false"/> deprecated rules will have severity as 'none'. If set to <see langword="true"/> they will treat as any other rule.</param>
    /// <returns>A list of rule severeties.</returns>
    internal IList<string> GetAssembliesRuleSevereties(string[] noneIds, string[] warningIds, bool addHeader = true, bool addSeparator = true, bool attendDeprecated = false)
    {
        var result = new List<string>();
        foreach (Assembly assembly in _assemblies)
        {
            IList<Rule> assemblyRuleSeverities = Helpers.GetAssemblyRuleSeverities(assembly);
            if (assemblyRuleSeverities.Count > 0)
            {
                if (result.Count is 0)
                {
                    result.AddRange(Constants.OutputFileHeader);
                }

                string assemblyRulesHeader = string.Format(CultureInfo.InvariantCulture, Constants.AssemblyRulesHeaderPattern, assembly.FullName);
                result.Add(assemblyRulesHeader);
                if (addSeparator)
                {
                    result.Add(string.Empty);
                }

                foreach (Rule rule in assemblyRuleSeverities)
                {
                    result.AddRange(GetRuleLines(noneIds, warningIds, addHeader, addSeparator, attendDeprecated, result, rule));
                }
            }
        }

        result.AddRange(Constants.OutputFileFooter);
        return result;
    }

    /// <summary>
    /// Loads the assembly.
    /// </summary>
    /// <param name="assemblyPath">The assembly path.</param>
    /// <param name="name">The name.</param>
    /// <param name="referenceAssembly">The reference assembly.</param>
    /// <returns>The loaded assembly, or an empty array when the assembly must be skipped.</returns>
    private static Assembly[] LoadAssembly(string assemblyPath, string name, Assembly referenceAssembly)
    {
        bool isSkipped = (referenceAssembly != null) ||
            string.IsNullOrWhiteSpace(name) ||
            name.EndsWith(Constants.ResourcesPattern, StringComparison.InvariantCulture) ||
            name.Contains(Constants.OtherLanguagePattern, StringComparison.InvariantCulture);
        return isSkipped
            ? []
            : [AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath)];
    }

    /// <summary>
    /// Gets the lines of a rule severity.
    /// </summary>
    /// <param name="noneIds">The none ids.</param>
    /// <param name="warningIds">The warning ids.</param>
    /// <param name="addHeader">If set to <see langword="true"/> a header will be added.</param>
    /// <param name="addSeparator">If set to <see langword="true"/> a separator will be added.</param>
    /// <param name="attendDeprecated">If set to <see langword="false"/> deprecated rules will have severity as 'none'. If set to <see langword="true"/> they will treat as any other rule.</param>
    /// <param name="ruleSevereties">The rule severeties already written.</param>
    /// <param name="rule">The rule.</param>
    /// <returns>The lines to add; empty when the rule severity is already present.</returns>
    private static List<string> GetRuleLines(string[] noneIds, string[] warningIds, bool addHeader, bool addSeparator, bool attendDeprecated, List<string> ruleSevereties, Rule rule)
    {
        string severityLevel = noneIds.Contains(rule.Id, StringComparer.Ordinal) ? Constants.NoneLevel : Constants.ErrorLevel;
        severityLevel = warningIds.Contains(rule.Id, StringComparer.Ordinal) ? Constants.WarningLevel : severityLevel;
        severityLevel = (!attendDeprecated && IsDeprecatedRule(rule)) ? Constants.NoneLevel : severityLevel;
        string ruleSeverity = string.Format(CultureInfo.InvariantCulture, Constants.RuleSeverityPattern, rule.Id, severityLevel);
        var lines = new List<string>();
        if (!ruleSevereties.Contains(ruleSeverity, StringComparer.Ordinal))
        {
            if (addHeader)
            {
                string ruleHeader = string.Format(CultureInfo.InvariantCulture, Constants.RuleHeaderPattern, rule.Id, rule.Title);
                lines.Add(ruleHeader.Trim());
            }

            lines.Add(ruleSeverity.Trim());
            if (addSeparator)
            {
                lines.Add(string.Empty);
            }
        }

        return lines;
    }

    /// <summary>
    /// Determines whether a rule is deprecated.
    /// </summary>
    /// <param name="rule">The rule.</param>
    /// <returns>
    ///   <see langword="true"/> If the rule is deprecated; otherwise, <see langword="false"/>.
    /// </returns>
    private static bool IsDeprecatedRule(Rule rule)
    {
        bool result = false;
        if (!string.IsNullOrWhiteSpace(rule.Title))
        {
            int index = 0;
            while ((index < Constants.DeprecatedTitlePatterns.Length) &&
                   !rule.Title.Contains(Constants.DeprecatedTitlePatterns[index], StringComparison.OrdinalIgnoreCase))
            {
                index++;
            }

            result = index < Constants.DeprecatedTitlePatterns.Length;
        }

        return result;
    }
}
