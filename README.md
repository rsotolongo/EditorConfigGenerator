# EditorConfigGenerator #

Sometimes is difficult to keep track of all code analyzers rules and their configured severity. Running this solution, an ".editorconfig" file is generated with few general settings and a list of all rules of all analyzers included in the solution.

The analyzers included by default in the solution are (via NuGet packages):

- Meziantou.Analyzer
- Microsoft.CodeAnalysis.CSharp.CodeStyle
- Microsoft.CodeAnalysis.NetAnalyzers
- Microsoft.CodeAnalysis.Workspaces.Common
- Roslynator.Analyzers
- SecurityCodeScan.VS2019
- SonarAnalyzer.CSharp
- StyleCop.Analyzers

By default all rules are configured by be reported as error but there are fer of them that are configured to be warnings and others to be ignored by the analyzers.

Rules with 'warning' severity:

- CA1056
- CA1716
- CA1724
- S1134
- S1135

Rules with 'none' severity (to be ignored):

- CS8019
- CS8933
- IDE0008
- IDE0130
- IDE0160
- RCS1002
- RCS1208 (seems to be a Visual Studio 2022 bug, will be removed eventually)
- S1309
- S1451
- S1694
- S3242
- S4018
- S4023
- SA1010
- SA1101
- SA1118
- SA1200
- SX1309
- SX1309S

Other rules with 'none' severity (to be ignored) due to they are deprecated:

- MA0018
- MA0038
- MA0041
- RCS1008
- RCS1009
- RCS1010
- RCS1012
- RCS1035
- RCS1038
- RCS1040
- RCS1041
- RCS1063
- RCS1064
- RCS1065
- RCS1066
- RCS1066FadeOut
- RCS1072
- RCS1091
- RCS1091FadeOut
- RCS1100
- RCS1101
- RCS1106
- RCS1176
- RCS1177
- RCS1208
- RCS1237

## How it works? ##

The solution needs to be downloaded and executed in order to generate the ".editorconfig" file in the working directory. Then, that file can be copied into the target solution to configure explicitly each code analyzer rule. There is no need to install all the same analyzers becuase the out-of-the-box rules are also included in the resulting file; but is something recommended to provide more clarity, stability, and security to the target source code.

This solution reads its own installed NuGet code analyzers packages (from the ".csproj" file) and using Reflection, extracts all rules, assembly per assembly. Before assign the severity of each rule, and based on corresponding constants, it verify if it should be ignored or treated as a warning. Those are the two main reasons why download and compile in own computer (PC or Mac) is needed to generate the ".editorconfig" file.

In case more analyzers are needed or different versions of the included ones, the solution needs modifications. Hopefully this generated file helps to build solutions with zero errors and zero warnings.
