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

The project also references `Microsoft.Net.Compilers.Toolset`. It is not an analyzer package: it pins the compiler to the version the CodeStyle analyzer needs. Without it, an SDK with an older compiler fails the build with `CS9057`. The generator skips it (`Constants.ExcludedPackages`). When you raise `Microsoft.CodeAnalysis.CSharp.CodeStyle`, raise the toolset to the same version.

## Severity rules

- Every rule is `error` by default.
- Rule IDs in `Constants.WarningIds` become `warning`.
- Rule IDs in `Constants.NoneIds` become `none`.
- Rules whose title contains a deprecation marker (`Constants.DeprecatedTitlePatterns`) become `none`.

All three lists live in [Constants.cs](EditorConfigGenerator/Constants.cs). Edit them there and regenerate.

## Output layout

1. `Constants.OutputFileHeader`: general settings (`charset = utf-8`, indentation) and the style, formatting and naming options.
2. One `# Rules from assembly: ...` section per analyzer assembly. Each rule gets a comment with its title and a `dotnet_diagnostic.<ID>.severity` line.

3. `Constants.OutputFileFooter`: the closing section, with the generated-code settings and the override for `*Controller.cs` files.

Files are stored as UTF-8 without a byte order mark: the header sets `charset = utf-8`, rule `SX1412` is `error` and its opposite `SA1412` is `none`.

## Usage

Build the project, then run it **from the build output folder**. The project is located relative to the working directory (`..\..\..`), so running from anywhere else, including `dotnet run` in the project folder, produces an empty file.

```bash
dotnet build
cd EditorConfigGenerator/bin/Debug/net10.0
dotnet EditorConfigGenerator.dll
```

The `.editorconfig` is written to that folder. Copy it into the solution you want to configure, or over this repository's own root `.editorconfig`. Installing the same analyzers there is recommended, but not required for the built-in rules to appear in the file.

## Releases

Every push to `main` publishes a GitHub release through [release.yml](.github/workflows/release.yml). Pushes that only change `*.md` files or `.gitignore` do not. The workflow builds the project, runs the generator and attaches the generated `.editorconfig` as the only release asset.

Release notes are generated from the commits and pull requests since the previous release. Download a release when you only need the config and don't want to build the generator yourself.

### Versioning

Versions follow [Semantic Versioning](https://semver.org) (`vMAJOR.MINOR.PATCH`). The workflow takes the latest `v*` tag and picks the bump from the commit message that landed on `main`. With squash merges, that is the pull request title.

| Commit message contains | Bump | Use for |
| --- | --- | --- |
| `#major` | `v1.2.3` → `v2.0.0` | Changes that break consumers, such as a different output layout |
| `#minor` | `v1.2.3` → `v1.3.0` | New analyzers, new severity behavior |
| neither | `v1.2.3` → `v1.2.4` | Analyzer package updates, changes to `WarningIds` or `NoneIds`, fixes |

The first release is `v1.0.0`. Every rule is `error` by default, so a new analyzer or a stricter severity can break consumers' builds. Treat those as at least a minor bump.

The release build runs with `TreatWarningsAsErrors`, so a stale `.editorconfig` in the repo root can fail it. See [Build notes](#build-notes) before merging changes to `NoneIds`, `WarningIds` or the header.

## How it works

1. `Helpers.GetFiles` reads the `PackageReference` entries from the project file and collects the DLLs of each package from `~/.nuget/packages`, except the packages in `Constants.ExcludedPackages`.
2. `Parser` loads those assemblies.
3. `Helpers.GetAssemblyRuleSeverities` instantiates each analyzer type with AutoFixture and reads its `SupportedDiagnostics` to get the rule IDs and titles. Types that cannot be built or whose property throws are skipped.
4. `Parser` assigns each rule its severity and writes the result.

Because it reads the packages from the local NuGet cache and uses reflection, the generator must be built and run on your own machine.

## Build notes

The project builds with `TreatWarningsAsErrors` and the generated `.editorconfig` in the repo root. After changing `NoneIds`, `WarningIds` or the header, the repo's `.editorconfig` may be stale and block the build. Build once with `-p:RunAnalyzers=false -p:EnforceCodeStyleInBuild=false`, run the generator, copy the output over the root `.editorconfig`, then build normally.

If the build fails with `CS9057` ("references version ... of the compiler, which is newer"), the `Microsoft.Net.Compilers.Toolset` version is older than an analyzer package. Raise it to match `Microsoft.CodeAnalysis.CSharp.CodeStyle`.

## Contributing

Contributions are welcome through pull requests. Nobody except the maintainer can push to `main`, so:

1. Fork the repository and create a branch in your fork.
2. Make your change. The build must finish with 0 errors and 0 warnings (see [Build notes](#build-notes)). Save files as UTF-8 without a byte order mark, or rule `SX1412` fails the build.
3. Open a pull request against `main`.

The maintainer reviews and approves every pull request before it is merged. Merging to `main` publishes a new release.

To report a security problem, use the repository's private vulnerability reporting (Security tab) instead of opening a public issue.
