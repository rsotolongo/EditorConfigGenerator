# CLAUDE.md

Project rules for EditorConfigGenerator. They take precedence over the global `~/.claude/CLAUDE.md` where they conflict (see "Overrides").

## What this is

.NET 10 console app (`EditorConfigGenerator.slnx`, one project). It reads its own `PackageReference` analyzer packages, loads their DLLs from `~/.nuget/packages`, reflects each `DiagnosticAnalyzer`'s `SupportedDiagnostics`, and writes an `.editorconfig` listing every rule with an explicit severity. A GitHub Actions workflow publishes that file as a release.

## Layout

| Path | Role |
| --- | --- |
| `EditorConfigGenerator/Program.cs` | Top-level statements: build `Parser`, write `Constants.OutputFilename` |
| `EditorConfigGenerator/Parser.cs` | Loads assemblies (`AssemblyLoadContext`), assigns severities |
| `EditorConfigGenerator/Helpers.cs` | Package/DLL discovery, analyzer instantiation (AutoFixture), rule extraction |
| `EditorConfigGenerator/Constants.cs` | All literals and the rule-ID lists |
| `EditorConfigGenerator/Rule.cs`, `VoidSpecimenContext.cs` | Rule model, AutoFixture helper |
| `EditorConfigGenerator/ConfigFiles/` | Companion files shipped to consumers (not generated) |
| `.editorconfig` (repo root) | The generated rules plus a hand-written head. Also governs this project's own build |
| `.github/workflows/release.yml` | Build, generate, package, tag, release on every push to `main` |

## Commands

```bash
dotnet build
cd EditorConfigGenerator/bin/Debug/net10.0 && dotnet EditorConfigGenerator.dll
```

- Run from `bin/Debug/net10.0`. The project folder is found via `Constants.RelativeWindowsPath` / `RelativeNonWindowsPath` (`../../..`) from the working directory. `dotnet run` from the project folder silently writes an empty `.editorconfig`.
- Build **Debug**, not Release. `DocumentationFile` is only set for Debug and the doc-file analyzers must pass (the workflow does the same).
- There are no tests. Verify by building with 0 errors and 0 warnings, running the generator, and checking the output is non-empty (about 10,600 lines) and that its first 73 lines match `OutputFileHeader`.

## Build is strict

`TreatWarningsAsErrors`, `AnalysisMode=All` and eight analyzers (Meziantou, Roslynator, Sonar, StyleCop, CodeStyle, NetAnalyzers, SecurityCodeScan) run in the build, with every rule at `error` except those in `Constants.NoneIds` / `WarningIds`. Fix the code to satisfy the rule. Do not use `[SuppressMessage]` or `#pragma`: `S1309` ("Do not suppress issues") is an error.

Rules that surprise people here:

- `RCS1078` with `roslynator_empty_string_style = field` requires `string.Empty`, not `""`.
- `IDE0007`/`IDE0008` follow `csharp_style_var_*`: `var` only when the type is apparent (`new`, casts), explicit type otherwise.
- Static readonly fields are PascalCase; private instance fields are `_camelCase`.
- `MA0217`: lambdas that capture nothing must be `static`.
- `RCS1208`: prefer early returns over nested `if`.
- `S3011`/`S3885`: no `BindingFlags.NonPublic`, no `Assembly.LoadFrom` (use `AssemblyLoadContext.Default.LoadFromAssemblyPath`).
- `S1192`: a literal repeated 3+ times becomes a constant (e.g. `Constants.SectionSeparator`).
- Public-facing members need XML docs, and every file starts with the copyright header block (company `RS`, from `StyleCop.json`). Match the existing files.

## The bootstrap trap

The repo-root `.editorconfig` is both generator output and the config this project is compiled under. After changing `NoneIds`, `WarningIds`, `DeprecatedTitlePatterns` or the header, the committed file is stale and can fail the build (for example `CS8933` in `obj/`). To recover:

1. `dotnet build -p:RunAnalyzers=false -p:EnforceCodeStyleInBuild=false`
2. Run the generator from `bin/Debug/net10.0`.
3. Replace the generated part of the root `.editorconfig` (everything from the first `# Rules from assembly:` line). The first 73 lines are the hand-written head, mirrored by `Constants.OutputFileHeader`; keep the two in sync.
4. Rebuild normally.

`Constants.OutputFileFooter` (the generated-code section) exists but is not appended to the output yet. It must be last in the file if it ever is.

## Conventions

- Rule-ID lists live only in `Constants.cs`. Do not duplicate them in the README or elsewhere.
- Adding an analyzer or changing a version means editing the `PackageReference` entries in the csproj; the generator discovers packages from there. `Microsoft.Bcl.AsyncInterfaces` is a deliberate runtime reference so analyzer assemblies load. Do not remove it.
- Analyzer types that cannot be built or whose `SupportedDiagnostics` throws (for example `SonarAnalyzer.ShimLayer` wrappers) are skipped on purpose.
- Companion files (`ConfigFiles/*`) are copied to the output folder and into the release zip. Their usage is documented in the README; update both together.
- Line endings: the repo is CRLF on Windows. Do not rewrite whole files to change endings.
- `EditorConfigGenerator.xml` (XML doc output) is tracked and changes on every Debug build. Do not commit it unless the diff is intentional.

## Releases

Every push to `main` that touches more than `*.md` or `.gitignore` creates a tagged GitHub release (patch bump by default; `#minor` or `#major` in the head commit message bumps the others). To push without releasing, put `[skip ci]` in the commit message. Pin changes to the workflow (runner image, action versions) are still pushes to `main`, so treat them the same way.

## Overrides of the global CLAUDE.md

- Comments: this project is XML-doc-heavy by analyzer requirement. Follow the surrounding `///` style on members; still add no explanatory `//` comments.
- Style: the surrounding file wins. Keep the existing `Helpers`/`Parser` shapes rather than introducing records, primary constructors or `Result<,>`; this is a small console tool.
- Source control is unchanged: no new branches, no commits or pushes unless asked, commits authored by the user with no Claude trailer.
