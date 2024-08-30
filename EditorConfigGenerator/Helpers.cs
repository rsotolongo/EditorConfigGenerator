//-----------------------------------------------------------------------
// <copyright file="Helpers.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using EditorConfigGenerator;
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
        var specimenContext = new VoidSpecimenContext();
        var fixture = new Fixture();
        object instance = default;
        if ((!type.IsAbstract) && !type.IsInterface && !IsSimple(type))
        {
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            instance = constructors?.Length > 0
                ? CreateInstance(fixture, specimenContext, type, constructors)
                : Activator.CreateInstance(type);
        }

        return instance;
    }

    /// <summary>
    /// Creates the instance.
    /// </summary>
    /// <param name="fixture">The fixture.</param>
    /// <param name="specimenContext">The specimen context.</param>
    /// <param name="type">The type.</param>
    /// <param name="constructors">The constructors.</param>
    /// <returns>The new instance.</returns>
    internal static object CreateInstance(ISpecimenBuilder fixture, ISpecimenContext specimenContext, Type type, ConstructorInfo[] constructors)
    {
        ParameterInfo[] constructorParameters = constructors[0].GetParameters();
        return constructorParameters?.Length > 0
            ? fixture.Create(type, specimenContext)
            : constructors[0].Invoke(parameters: null);
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
            foreach (Type type in assembly.GetTypes().Where(item => !item.IsAbstract).ToArray())
            {
                List<PropertyInfo> properties = [.. type.GetProperties()];
                PropertyInfo supportedDiagnosticsProperty = properties.Find(item => string.Equals(item.Name, Constants.SupportedDiagnosticsPropertyName, StringComparison.Ordinal));
                IList<Rule> typeRules = FindTypeRules(type, supportedDiagnosticsProperty);
                result.AddRange(typeRules);
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
        List<string> filePaths = [.. Directory.GetFiles(projectDirectory)];
        string projectPath = filePaths.Find(item => item.EndsWith(Constants.ProjectExtension, StringComparison.InvariantCulture));
        if (File.Exists(projectPath))
        {
            XElement projectReference = XElement.Load(projectPath);
            if (projectReference is not null)
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
            var typeInfo = type.GetTypeInfo();
            if ((typeInfo?.IsGenericType is true) && (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>)))
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
    /// Creates the type rule.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="supportedDiagnostic">The supported diagnostic.</param>
    private static void CreateTypeRule(List<Rule> result, DiagnosticDescriptor supportedDiagnostic)
    {
        if (supportedDiagnostic is not null)
        {
            var rule = new Rule
            {
                Id = supportedDiagnostic.Id,
                Title = supportedDiagnostic.Title.ToString(CultureInfo.InvariantCulture),
            };
            Rule existingRule = result.Find(item => string.Equals(item.Id, rule.Id, StringComparison.Ordinal));
            if (existingRule is null)
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
            var supportedDiagnostics = (propertyValue is not null)
                ? (IEnumerable<DiagnosticDescriptor>)supportedDiagnosticsProperty.GetValue(instance)
                : default;
            if (supportedDiagnostics is not null)
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
        if (assemblyFiles is not null)
        {
            result.AddRange(assemblyFiles);
        }

        return result;
    }
}
