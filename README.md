# EditorConfigGenerator

Keeping track of every code analyzer rule and its severity is hard. EditorConfigGenerator is a .NET 10 console app that reads the analyzer packages referenced by its own project and writes an `.editorconfig` that lists every rule of every analyzer with an explicit severity.

## Analyzers covered

The rules come from the NuGet analyzer packages referenced in [EditorConfigGenerator.csproj](EditorConfigGenerator/EditorConfigGenerator.csproj):

- Meziantou.Analyzer
- Microsoft.CodeAnalysis.CSharp.CodeStyle
- Microsoft.CodeAnalysis.NetAnalyzers
- Microsoft.CodeAnalysis.Workspaces.Common
- Roslynator.Analyzers
- SecurityCodeScan.VS2019
- SonarAnalyzer.CSharp
- StyleCop.Analyzers

To cover other analyzers, or other versions of these, edit the `PackageReference` entries and rebuild.

## Severity rules

- Every rule is `error` by default.
- Rule IDs in `Constants.WarningIds` become `warning`.
- Rule IDs in `Constants.NoneIds` become `none`.
- Rules whose title contains a deprecation marker (`Constants.DeprecatedTitlePatterns`) become `none`.

All three lists live in [Constants.cs](EditorConfigGenerator/Constants.cs). Edit them there and regenerate.

## Output layout

1. `Constants.OutputFileHeader`: general settings and the hand-written style, formatting and naming options.
2. One `# Rules from assembly: ...` section per analyzer assembly. Each rule gets a comment with its title and a `dotnet_diagnostic.<ID>.severity` line.

`Constants.OutputFileFooter` defines the closing section for generated code. It is not appended to the output yet.

## Usage

Build the project, then run it **from the build output folder**. The project is located relative to the working directory (`..\..\..`), so running from anywhere else, including `dotnet run` in the project folder, produces an empty file.

```bash
dotnet build
cd EditorConfigGenerator/bin/Debug/net10.0
dotnet EditorConfigGenerator.dll
```

The `.editorconfig` is written to that folder. Copy it into the solution you want to configure. Installing the same analyzers there is recommended, but not required for the built-in rules to appear in the file.

## Companion files

The [ConfigFiles](EditorConfigGenerator/ConfigFiles) folder holds files to copy into the target solution next to the generated `.editorconfig`. The generated file applies to every `*.cs` file. These files relax a few rules for specific kinds of project.

### Per-project `.editorconfig` overrides

Copy the file that matches the project type into that project's folder and name it `.editorconfig`. EditorConfig applies the closest file last, so these override the root file for that project only. Do not set `root = true` in them.

| File | Use in | Rules set to `none` |
| --- | --- | --- |
| `app-project.editorconfig` | Each application project (API, worker, console) | `CA2007`; `CS1591` only if the app is not consumed as a library |
| `web-project.editorconfig` | Each ASP.NET Core web project (controllers must stay public) | `CA2007`, `CA1515`, `CS1591` |
| `test-project.editorconfig` | Each test project | `CA2007`, `CA1707`, `CA1515`, `CS1591` |

Library projects need no override.

### `CodeMetricsConfig.txt`

Sets the thresholds for the code metrics rules `CA1502` (cyclomatic complexity), `CA1505` (maintainability index) and `CA1506` (class coupling, with separate method and type limits). Copy it to the root of each project that should use these thresholds and register it as an additional file:

```xml
<ItemGroup>
  <AdditionalFiles Include="CodeMetricsConfig.txt" />
</ItemGroup>
```

The file name must stay exactly `CodeMetricsConfig.txt`, or the analyzer ignores it. Without it, the analyzer uses its default thresholds.

### `StyleCop.json`

Settings for StyleCop.Analyzers. Register it the same way, as `<AdditionalFiles Include="StyleCop.json" />`. This project already does so from `ConfigFiles\StyleCop.json`.

## Releases

Every push to `main` publishes a GitHub release through [release.yml](.github/workflows/release.yml). Pushes that only change `*.md` files or `.gitignore` do not. The workflow builds the project, runs the generator and attaches two assets:

- `EditorConfigGenerator-vX.Y.Z.zip`: the generated `.editorconfig` plus the [companion files](#companion-files).
- `.editorconfig`: the generated file alone.

Release notes are generated from the commits and pull requests since the previous release. Download a release when you only need the config and don't want to build the generator yourself.

### Versioning

Versions follow [Semantic Versioning](https://semver.org) (`vMAJOR.MINOR.PATCH`). The workflow takes the latest `v*` tag and picks the bump from the commit message that landed on `main`. With squash merges, that is the pull request title.

| Commit message contains | Bump | Use for |
| --- | --- | --- |
| `#major` | `v1.2.3` → `v2.0.0` | Changes that break consumers, such as a renamed or removed companion file or a different output layout |
| `#minor` | `v1.2.3` → `v1.3.0` | New analyzers, new companion files, new severity behavior |
| neither | `v1.2.3` → `v1.2.4` | Analyzer package updates, changes to `WarningIds` or `NoneIds`, fixes |

The first release is `v1.0.0`. Every rule is `error` by default, so a new analyzer or a stricter severity can break consumers' builds. Treat those as at least a minor bump.

The release build runs with `TreatWarningsAsErrors`, so a stale `.editorconfig` in the repo root can fail it. See [Build notes](#build-notes) before merging changes to `NoneIds`, `WarningIds` or the header.

## How it works

1. `Helpers.GetFiles` reads the `PackageReference` entries from the project file and collects the DLLs of each package from `~/.nuget/packages`.
2. `Parser` loads those assemblies.
3. `Helpers.GetAssemblyRuleSeverities` instantiates each analyzer type with AutoFixture and reads its `SupportedDiagnostics` to get the rule IDs and titles. Types that cannot be built or whose property throws are skipped.
4. `Parser` assigns each rule its severity and writes the result.

Because it reads the packages from the local NuGet cache and uses reflection, the generator must be built and run on your own machine.

## Build notes

The project builds with `TreatWarningsAsErrors` and the generated `.editorConfig` in the repo root. After changing `NoneIds`, `WarningIds` or the header, the repo's `.editorConfig` may be stale and block the build. Build once with `-p:RunAnalyzers=false -p:EnforceCodeStyleInBuild=false`, regenerate the file, then build normally.
