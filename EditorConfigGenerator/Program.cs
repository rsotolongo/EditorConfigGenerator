//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace EditorConfig;

/// <summary>
/// Entry point implementation.
/// </summary>
public static class Program
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    public static void Main()
    {
        var parser = new Parser();
        IList<string> assembliesRuleSeverities = parser.GetAssembliesRuleSevereties(Constants.NoneIds, Constants.WarningIds);
        File.WriteAllLines(Constants.OutputFilename, assembliesRuleSeverities);
    }
}
