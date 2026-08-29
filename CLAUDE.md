# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A .NET 8, **Windows-only** console tool that bulk-converts image folders between DDS, PNG, TGA and JPEG, with hash-based duplicate detection persisted to a `Result.json` file. Interactive CLI built on Spectre.Console. All image encode/decode goes through the native **DirectXTex** library (`Hexa.NET.DirectXTex`), which is why the tool is Windows-only and the projects pin `RuntimeIdentifier=win-x64`.

## Build / test

The solution file is `DDS.Tools.slnx` (XML solution format — pass it explicitly to `dotnet`).

```pwsh
dotnet restore DDS.Tools.slnx
dotnet build DDS.Tools.slnx --configuration Release --no-restore
dotnet test DDS.Tools.slnx --configuration Release
```

Run a single test:

```pwsh
dotnet test DDS.Tools.slnx --filter "FullyQualifiedName~TodoServiceTests.GetTodosFilesFoundReturnsTodos"
```

Run the CLI:

```pwsh
dotnet run --project src/DDS.Tools -- convert "D:\DDS-Textures" "D:\PNG-Textures" --to PNG
```

### Build constraints that will bite you

- `TreatWarningsAsErrors` and `EnforceCodeStyleInBuild` are on. Analyzer/style violations fail the build.
- Every `.cs` file must start with the MIT license header block (see `DDS.Tools.slnx.licenseheader`). `.editorconfig` enforces tabs, CRLF, and **no** final newline for `.cs`.
- Central package management: add/bump versions in `Directory.Packages.props`, never in the `.csproj`.
- Assembly version is derived from UTC date/time in `Directory.Build.props` — do not hand-edit version numbers.
- `Debug` builds get a `Development` version suffix and run the logger at `Debug` level; `Release` strips docs and PDBs.
- `AllowUnsafeBlocks` is on — the codec layer uses pointers to talk to DirectXTex.
- Both projects set `RuntimeIdentifier=win-x64` so the native DirectXTex binary is always restored/copied.

## Architecture

### Composition

`Program.Main` builds a generic `IHost` (`Microsoft.Extensions.Hosting`) for DI, then hands it to a Spectre.Console.Cli `CommandApp` through the `TypeRegistrar` / `TypeResolver` bridge (`Common/`). Services register in `Extensions/ServiceCollectionExtensions.RegisterServices`; commands register in `Extensions/ConfiguratorExtensions.ConfigureCommands`. One command: `convert`, bound to `ConvertSettings` (`<SourceFolder> <TargetFolder> [ConvertMode]`, `--from`/`--to`, `--retain`/`--bysize`, `--compression`). `--from` is optional; `ConvertCommand.InferSourceFormat` scans the source folder and picks the one present format, erroring on none or several.

### Conversion pipeline

`ConvertCommand.Action` → `ITodoService`, which orchestrates three internal services (constructed directly, not via DI). Source and target formats travel on `ConvertSettings` (`SourceFormat` throws if `--from` was never resolved):

1. **`TodoPlanningService`** — builds a `TodoCollection` (a `ConcurrentQueue<TodoModel>`). If `Result.json` exists in the *source* folder it maps entries from that JSON; otherwise it globs the source folder for every extension of `settings.SourceFormat` and MD5-hashes each file's bytes (no decode).
2. **`TodoTransformationService`** — iterates todos, skips hash duplicates (mode-dependent), and either re-encodes via the codec layer or, in `Grouping` mode, copies the source file untouched. Output files keep the old naming: `<hash>.<TARGET-ENUM-NAME>` (e.g. `A1B2.PNG`).
3. **`TodoPersistenceService`** — writes `Result.json` to the *target* folder (Automatic mode only).

`Result.json` is the cross-run dedup ledger. Only `FileName`, `RelativePath`, `FileHash` are serialized (`FullPathName` / `TargetFolder` are `[JsonIgnore]`). Read from source, written to target — so a second run pointed at the previous target skips already-converted files.

### Convert modes (`ConvertModeType`)

| Mode | Dedup by hash | Target subfolder | Output filename | `Result.json` written | Re-encode |
| --- | --- | --- | --- | --- | --- |
| `Automatic` (default) | yes | `<width>` | file hash | yes | yes |
| `Manual` | no | honors `--retain` / `--bysize` | hash, or original name when `--retain` | no | yes |
| `Grouping` | yes | `<width>x<height>` | original name | no | no — raw copy |

`--retain` / `--bysize` options only apply to `Manual`.

### Codec layer (`Imaging/`, `Interfaces/Imaging/`)

Decode and encode are split and per-format so a new format is just an enum value plus a codec registration:

- `ImageCanvas` — the neutral pivot: `Width`, `Height`, tightly-packed top-down RGBA32 `byte[]`. Plain managed memory, no disposal.
- `IImageDecoder` (`Decode(byte[]) → ImageCanvas`) and `IImageEncoder` (`Encode(ImageCanvas, CompressionLevel) → byte[]`), each carrying its `Format`. Stateless.
- `IImageCodecRegistry` / `ImageCodecRegistry` — resolves the decoder/encoder for an `ImageType`, throwing `CommandException` when a direction is unsupported.
- `DirectXTexCodec` — one instance per format, implements both interfaces over `Hexa.NET.DirectXTex`. Decode: `LoadFrom{DDS,TGA,WIC}Memory` (WIC covers PNG + JPG) → `Decompress`/`Convert` to `R8G8B8A8_UNORM` → copy rows out. Encode DDS: `Initialize2D` + row copy → `GenerateMipMaps2` → `Compress2` to BC1 (opaque) / BC3 (alpha) → `SaveToDDSMemory2`. Encode TGA: `SaveToTGAMemory`. Encode PNG/JPG: `SaveToWICMemory` with the codec GUID. `CompressionLevel` maps to `TexCompressFlags` — approximate, since WIC/BC knobs are coarser than the old ones. Every native `ScratchImage`/`Blob` is released before returning; gotcha: `Convert` fails with `E_INVALIDARG` if source format already equals the target, so it is skipped in that case.
- Supported formats are the `SupportedFormats` array in `ServiceCollectionExtensions` (`DDS`, `PNG`, `TGA`, `JPG`); one `DirectXTexCodec` is registered as both `IImageDecoder` and `IImageEncoder` per entry. To add a format: extend `ImageType`, its `ImageTypeExtensions` extension lists, the `DirectXTexCodec` `Load`/`Encode` switches, and `SupportedFormats`; drop fixtures under `tests/.../Resources/<FMT>/` with matching `<None Update>` csproj entries.
- Hash is the MD5 of the raw source bytes (`TodoPlanningService`); it is both the dedup key and the default output filename. JPEG drops alpha, so an RGB-identical pair of sources (e.g. the `32.jpg` / `32A.jpg` fixtures) hash the same and dedup as one.

### Providers

`DirectoryProvider`, `FileProvider`, `PathProvider` are thin abstractions over `System.IO.Directory` / `File` / `Path`, generated by `BB84.SourceGenerators` `[GenerateAbstraction(...)]` on `partial` classes. This exists purely to make file-system access mockable in tests. Generated sources land in `obj/Generated/` (`EmitCompilerGeneratedFiles`).

### Logging

`ILoggerService<T>` / `LoggerService<T>` wrap `ILogger<T>` and take pre-built `LoggerMessage.Define` delegates. Services catch exceptions, log via this wrapper, and rethrow as `ServiceException`; commands catch and translate to a non-zero exit code with a red console line. `CommandException` is thrown for user errors (e.g. missing source folder).

## Tests

MSTest 4.x + Moq. `internal` types are visible via `InternalsVisibleTo` (`Directory.Build.props`).

- `UnitTestBase` spins up a **real** DI host once (`[AssemblyInitialize]`) exposed as `ServiceProvider`; `TestConstants` holds the `Resources/` fixture paths (real `.dds` / `.png` / `Result.json` files copied to output).
- Service tests mock the providers plus `IImageCodecRegistry` / `IImageDecoder` / `IImageEncoder`. `DirectXTexCodecTests` exercises the real codec against the fixtures.
- Use the modern assertion API already in use here: `Assert.IsEmpty`, `Assert.HasCount`, `Assert.IsGreaterThan` / `Assert.IsLessThan`, `Assert.Throws<T>`, `[DataRow(..., DisplayName = ...)]`.
