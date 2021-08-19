//-----------------------------------------------------------------------
// <copyright file="Helpers.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace EditorConfig
{
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
            if ((type != null) && !type.IsAbstract && !type.IsInterface && !IsSimple(type))
            {
                ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if ((constructors != null) && (constructors.Length > 0))
                {
                    ParameterInfo[] constructorParameters = constructors[0].GetParameters();
                    if ((constructorParameters != null) && (constructorParameters.Length > 0))
                    {
                        IList<object> parameterInstances = CreateParameterInstances(constructorParameters);
                        instance = constructors[0].Invoke(parameterInstances.ToArray());
                    }
                    else
                    {
                        instance = constructors[0].Invoke(null);
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
                        PropertyInfo supportedDiagnosticsProperty = properties.FirstOrDefault(item => item.Name == Constants.SupportedDiagnosticsPropertyName);
                        IList<Rule> typeRules = FindTypeRules(type, supportedDiagnosticsProperty);
                        result.AddRange(typeRules);
                    }
                }

                result = result.OrderBy(item => item.Id).ToList();
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
            string relativePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Constants.RelativeWindowsPath
                : Constants.RelativeNonWindowsPath;
            string projectDirectory = Path.Combine(currentDirectory, relativePath);
            string projectPath = Directory.GetFiles(projectDirectory).FirstOrDefault(item => item.EndsWith(Constants.ProjectExtension, StringComparison.InvariantCulture));
            if (File.Exists(projectPath))
            {
                XElement purchaseOrder = XElement.Load(projectPath);
                if (purchaseOrder != null)
                {
                    string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string packagesFolder = Path.Combine(userProfileFolder, Constants.PackagesDirectoryName, Constants.PackagesSubdirectoryName);
                    IEnumerable<XElement> packageReferences = purchaseOrder.Descendants(Constants.PackageNodeName);
                    foreach (XElement package in packageReferences)
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

            return result.ToArray();
        }

        /// <summary>
        /// Determines whether the specified type is simple.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is simple; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsSimple(Type type)
        {
            bool result = false;
            if (type != null)
            {
                System.Reflection.TypeInfo typeInfo = type.GetTypeInfo();
                if ((typeInfo != null) && typeInfo.IsGenericType && (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>)))
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
        private static IList<object> CreateParameterInstances(ParameterInfo[] constructorParameters)
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
                Rule existingRule = result.FirstOrDefault(item => item.Id == rule.Id);
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
        private static IList<Rule> FindTypeRules(Type type, PropertyInfo supportedDiagnosticsProperty)
        {
            var result = new List<Rule>();
            if (supportedDiagnosticsProperty != null)
            {
                object instance = CreateType(type);
                object propertyValue = (instance != null)
                    ? supportedDiagnosticsProperty.GetValue(instance)
                    : default;
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
        private static IList<string> GetFiles(string[] assemblyFiles)
        {
            var result = new List<string>();
            if (assemblyFiles != null)
            {
                foreach (string assemblyFile in assemblyFiles)
                {
                    result.Add(assemblyFile);
                }
            }

            return result;
        }
    }
}
