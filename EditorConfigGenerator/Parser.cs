//-----------------------------------------------------------------------
// <copyright file="Parser.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Globalization;
using System.Reflection;

namespace EditorConfig;

/// <summary>
/// Parser implementation.
/// </summary>
internal sealed class Parser
{
    private readonly Assembly[] assemblies;

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
        if (assemblyPaths != null)
        {
            foreach (string path in assemblyPaths.Where(File.Exists))
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);
                string fullName = assemblyName?.FullName;
                string name = assemblyName?.Name;
                Assembly referenceAssembly = assembliesList.Find(item => string.Equals(item.FullName, fullName, StringComparison.Ordinal));
                AddAssembly(assembliesList, path, name, referenceAssembly);
            }
        }

        assemblies = [.. assembliesList];
    }

    /// <summary>
    /// Gets the assemblies rule severeties.
    /// </summary>
    /// <param name="noneIds">The rules identifiers to ignore.</param>
    /// <param name="warningIds">The rules identifiers to warn about.</param>
    /// <param name="addHeader">if set to <see langword="true"/> [add header].</param>
    /// <param name="addSeparator">if set to <see langword="true"/> [add separator].</param>
    /// <returns>A list of rule severeties.</returns>
    internal IList<string> GetAssembliesRuleSevereties(string[] noneIds, string[] warningIds, bool addHeader = true, bool addSeparator = true)
    {
        var result = new List<string>();
        foreach (Assembly assembly in assemblies)
        {
            IList<Rule> assemblyRuleSeverities = Helpers.GetAssemblyRuleSeverities(assembly);
            if (assemblyRuleSeverities.Count > 0)
            {
                if (result.Count == 0)
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
                    AddRuleSeverety(noneIds, warningIds, addHeader, addSeparator, result, rule);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Adds the assembly.
    /// </summary>
    /// <param name="assembliesList">The assemblies list.</param>
    /// <param name="assemblyPath">The assembly path.</param>
    /// <param name="name">The name.</param>
    /// <param name="referenceAssembly">The reference assembly.</param>
    private static void AddAssembly(List<Assembly> assembliesList, string assemblyPath, string name, Assembly referenceAssembly)
    {
        if ((referenceAssembly == null) &&
            !string.IsNullOrWhiteSpace(name) &&
            !name.EndsWith(Constants.ResourcesPattern, StringComparison.InvariantCulture) &&
            !name.Contains(Constants.OtherLanguagePattern, StringComparison.InvariantCulture))
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            if (assembly != null)
            {
                assembliesList.Add(assembly);
            }
        }
    }

    /// <summary>
    /// Adds the rule severety.
    /// </summary>
    /// <param name="noneIds">The none ids.</param>
    /// <param name="warningIds">The warning ids.</param>
    /// <param name="addHeader">If set to <see langword="true"/> a header will be added.</param>
    /// <param name="addSeparator">If set to <see langword="true"/> a separator will be added.</param>
    /// <param name="ruleSevereties">The rule severeties.</param>
    /// <param name="rule">The rule.</param>
    private static void AddRuleSeverety(string[] noneIds, string[] warningIds, bool addHeader, bool addSeparator, List<string> ruleSevereties, Rule rule)
    {
        string severityLevel = noneIds.Contains(rule.Id, StringComparer.Ordinal) ? Constants.NoneLevel : Constants.ErrorLevel;
        severityLevel = warningIds.Contains(rule.Id, StringComparer.Ordinal) ? Constants.WarningLevel : severityLevel;
        string ruleSeverity = string.Format(CultureInfo.InvariantCulture, Constants.RuleSeverityPattern, rule.Id, severityLevel);
        if (!ruleSevereties.Contains(ruleSeverity, StringComparer.Ordinal))
        {
            if (addHeader)
            {
                string ruleHeader = string.Format(CultureInfo.InvariantCulture, Constants.RuleHeaderPattern, rule.Id, rule.Title);
                ruleSevereties.Add(ruleHeader.Trim());
            }

            ruleSevereties.Add(ruleSeverity.Trim());
            if (addSeparator)
            {
                ruleSevereties.Add(string.Empty);
            }
        }
    }
}
