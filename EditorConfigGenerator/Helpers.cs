//-----------------------------------------------------------------------
// <copyright file="Helpers.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace EditorConfig;

/// <summary>
/// Global routines.
/// </summary>
internal static class Helpers
{
    /// <summary>
    /// Creates the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A type.</returns>
    internal static object CreateType(Type type)
    {
        object instance = default;
        if ((type?.IsAbstract == false) && !type.IsInterface && !IsSimple(type))
        {
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (constructors?.Length > 0)
            {
                ParameterInfo[] constructorParameters = constructors[0].GetParameters();
                if (constructorParameters?.Length > 0)
                {
                    IList<object> parameterInstances = CreateParameterInstances(constructorParameters);
                    instance = constructors[0].Invoke([.. parameterInstances]);
                }
                else
                {
                    instance = constructors[0].Invoke(parameters: null);
                }
            }
            else
            {
                instance = Activator.CreateInstance(type);
            }
        }

        return instance;
    }

    /// <summary>
    /// Gets the assembly rule severities.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>A list of rules.</returns>
    internal static IList<Rule> GetAssemblyRuleSeverities(Assembly assembly)
    {
        var result = new List<Rule>();
        if (assembly != null)
        {
            Type[] types = assembly.GetTypes();
            if (types != null)
            {
                foreach (Type type in types)
                {
                    PropertyInfo[] properties = type.GetProperties();
                    PropertyInfo supportedDiagnosticsProperty = properties.Find(item => string.Equals(item.Name, Constants.SupportedDiagnosticsPropertyName, StringComparison.Ordinal));
                    IList<Rule> typeRules = FindTypeRules(type, supportedDiagnosticsProperty);
                    result.AddRange(typeRules);
                }
            }

            result = [.. result.OrderBy(item => item.Id, StringComparer.Ordinal)];
        }

        return result;
    }

    /// <summary>
    /// Gets the files.
    /// </summary>
    /// <returns>A list of files.</returns>
    internal static string[] GetFiles()
    {
        var result = new List<string>();
        string currentDirectory = Directory.GetCurrentDirectory();
        string relativePath = OperatingSystem.IsWindows()
            ? Constants.RelativeWindowsPath
            : Constants.RelativeNonWindowsPath;
        string projectDirectory = Path.Combine(currentDirectory, relativePath);
        string projectPath = Directory.GetFiles(projectDirectory).Find(item => item.EndsWith(Constants.ProjectExtension, StringComparison.InvariantCulture));
        if (File.Exists(projectPath))
        {
            XElement projectReference = XElement.Load(projectPath);
            if (projectReference != null)
            {
                string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string packagesFolder = Path.Combine(userProfileFolder, Constants.PackagesDirectoryName, Constants.PackagesSubdirectoryName);
                foreach (XElement package in projectReference.Descendants(Constants.PackageNodeName))
                {
                    XAttribute packageNameAttribute = package.Attributes().FirstOrDefault(item => item.Name == Constants.PackageNameAttribute);
                    string packageName = !string.IsNullOrWhiteSpace(packageNameAttribute?.Value)
                        ? packageNameAttribute.Value.Trim()
                        : string.Empty;
                    XAttribute packageVersionAttribute = package.Attributes().FirstOrDefault(item => item.Name == Constants.PackageVersionAttribute);
                    string packageVersion = !string.IsNullOrWhiteSpace(packageVersionAttribute?.Value)
                        ? packageVersionAttribute.Value.Trim()
                        : string.Empty;
                    string folder = Path.Combine(packagesFolder, packageName, packageVersion);
                    string[] assemblyFiles = Directory.GetFiles(folder, Constants.AssembliesPattern, SearchOption.AllDirectories);
                    IList<string> files = GetFiles(assemblyFiles);
                    result.AddRange(files);
                }
            }
        }

        return [.. result];
    }

    /// <summary>
    /// Determines whether the specified type is simple.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///   <see langword="true"/> if the specified type is simple; otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool IsSimple(Type type)
    {
        bool result = false;
        if (type != null)
        {
            System.Reflection.TypeInfo typeInfo = type.GetTypeInfo();
            if ((typeInfo?.IsGenericType == true) && (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                result = IsSimple(typeInfo.GetGenericArguments()[0]);
            }

            if (typeInfo != null)
            {
                result = result
                    || typeInfo.IsPrimitive
                    || typeInfo.IsEnum;
            }

            result = result
                || type.Equals(typeof(string))
                || type.Equals(typeof(decimal));
        }

        return result;
    }

    /// <summary>
    /// Creates the parameter instances.
    /// </summary>
    /// <param name="constructorParameters">The constructor parameters.</param>
    /// <returns>A list of parameter instances.</returns>
    private static List<object> CreateParameterInstances(ParameterInfo[] constructorParameters)
    {
        var result = new List<object>();
        if (constructorParameters != null)
        {
            foreach (ParameterInfo constructorParameter in constructorParameters)
            {
                object value = CreateType(constructorParameter.ParameterType);
                result.Add(value);
            }
        }

        return result;
    }

    /// <summary>
    /// Creates the type rule.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="supportedDiagnostic">The supported diagnostic.</param>
    private static void CreateTypeRule(List<Rule> result, DiagnosticDescriptor supportedDiagnostic)
    {
        if (supportedDiagnostic != null)
        {
            var rule = new Rule
            {
                Id = supportedDiagnostic.Id,
                Title = supportedDiagnostic.Title.ToString(CultureInfo.InvariantCulture),
            };
            Rule existingRule = result.Find(item => string.Equals(item.Id, rule.Id, StringComparison.Ordinal));
            if (existingRule == null)
            {
                result.Add(rule);
            }
        }
    }

    /// <summary>
    /// Finds the type rules.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="supportedDiagnosticsProperty">The supported diagnostics property.</param>
    private static List<Rule> FindTypeRules(Type type, PropertyInfo supportedDiagnosticsProperty)
    {
        var result = new List<Rule>();
        if (supportedDiagnosticsProperty != null)
        {
            object instance = CreateType(type);
            object propertyValue = supportedDiagnosticsProperty.GetValue(instance);
            var supportedDiagnostics = (propertyValue != null)
                ? (IEnumerable<DiagnosticDescriptor>)supportedDiagnosticsProperty.GetValue(instance)
                : default;
            if (supportedDiagnostics != null)
            {
                foreach (DiagnosticDescriptor supportedDiagnostic in supportedDiagnostics)
                {
                    CreateTypeRule(result, supportedDiagnostic);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the files.
    /// </summary>
    /// <param name="assemblyFiles">The assembly files.</param>
    private static List<string> GetFiles(string[] assemblyFiles)
    {
        var result = new List<string>();
        if (assemblyFiles != null)
        {
            result.AddRange(assemblyFiles);
        }

        return result;
    }
}
