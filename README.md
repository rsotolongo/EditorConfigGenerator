# EditorConfigGenerator #

Sometimes is difficult to keep track of all code analyzers rules and their configured severity. Running this solution, an ".editorconfig" file is generated with few general settings and a list of all rules of all analyzers included in the solution.

The analyzers included by default in the solution are (via NuGet packages):

- Microsoft.CodeAnalysis.CSharp.CodeStyle
- Microsoft.CodeAnalysis.NetAnalyzers
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
- IDE0008
- IDE0130
- S1309
- S1451
- S1694
- S4018
- S4023
- SA1101
- SA1118
- SA1200
- SX1309
- SX1309S

## How it works? ##

The solution needs to be downloaded and executed in order to generate the ".editorconfig" file in the working directory. Then, that file can be copied into the target solution to configure explicitly each code analyzer rule. There is no need to install all the same analyzers becuase the out-of-the-box rules are also included in the resulting file; but is something recommended to provide more clarity, stability, and security to the target source code.

This solution reads its own installed NuGet code analyzers packages (from the ".csproj" file) and using Reflection, extracts all rules, assembly per assembly. Before assign the severity of each rule, and based on corresponding constants, it verify if it should be ignored or treated as a warning. Those are the two main reasons why download and compile in own computer (PC or Mac) is needed to generate the ".editorconfig" file.

In case more analyzers are needed or different versions of the included ones, the solution needs modifications. Hopefully this generated file helps to build solutions with zero errors and zero warnings.
