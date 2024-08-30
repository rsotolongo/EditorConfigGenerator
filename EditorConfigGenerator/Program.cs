//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="RS">
//     Copyright (c). All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using EditorConfigGenerator;

var parser = new Parser();

IList<string> assembliesRuleSeverities = parser.GetAssembliesRuleSevereties(Constants.NoneIds, Constants.WarningIds);

await File.WriteAllLinesAsync(Constants.OutputFilename, assembliesRuleSeverities, CancellationToken.None).ConfigureAwait(false);
