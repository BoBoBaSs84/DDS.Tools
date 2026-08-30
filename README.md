# DDS.Tools

I needed some tools that were able to do some slight lifting so I can do some modding. This is a simple DDS and PNG tool set that converts DDS images to PNG images and vice versa on a large scale. It has options for duplicate detection and sorting.

[![CI](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/ci.yml)
[![CD](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/cd.yml/badge.svg?event=push)](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/cd.yml)
[![CodeQL](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/github-code-scanning/codeql/badge.svg?branch=main)](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/github-code-scanning/codeql)
[![Dependabot](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/dependabot/dependabot-updates/badge.svg?branch=main)](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/dependabot/dependabot-updates)

[![.NET](https://img.shields.io/badge/net8.0-5C2D91?logo=.NET&labelColor=gray)](https://github.com/BoBoBaSs84/DDS.Tools)
[![C#](https://img.shields.io/badge/C%23-13.0-239120)](https://github.com/BoBoBaSs84/DDS.Tools)
[![Issues](https://img.shields.io/github/issues/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/issues)
[![Commit](https://img.shields.io/github/last-commit/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/commit/main)
[![Size](https://img.shields.io/github/repo-size/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools)
[![License](https://img.shields.io/github/license/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/blob/main/LICENSE)
[![Release](https://img.shields.io/github/v/release/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/releases/latest)

## Features

- Convert whole folders of textures between **DDS**, **PNG**, **TGA** and **JPEG** on a large scale
- Source format is auto-detected from the folder, or set explicitly with `--from`
- Folders may hold a **mix of formats** &mdash; every recognized file is converted in one pass
- **Round-trip** support: `--restore` converts edited files back to each file's original format
- Duplicate detection via a persisted result JSON file
- Optional **retain structure** mode to preserve original folder and file names
- Optional **separate by size** mode to sort textures into sub-folders by resolution
- Configurable compression effort for the written images
- Three convert modes: `Automatic`, `Manual`, and `Grouping`
- Interactive CLI with progress spinner powered by [Spectre.Console](https://spectreconsole.net/)

Image encoding and decoding is handled by [DirectXTex](https://github.com/microsoft/DirectXTex), so DDS.Tools is a **Windows-only** tool.

## Usage

DDS.Tools convert <SourceFolder> <TargetFolder> [ConvertMode] --to <format> [options]

### Arguments

| Argument         | Description                                               |
| ---------------- | --------------------------------------------------------- |
| `<SourceFolder>` | Path to the folder containing the source images           |
| `<TargetFolder>` | Path to the folder where converted images will be written |
| `[ConvertMode]`  | Optional. Convert mode to use. Default is `Automatic`     |

### Options

| Option                | Description                                                                                             |
| --------------------- | ------------------------------------------------------------------------------------------------------- |
| `-f`, `--from`        | Source image format (`DDS`, `PNG`, `TGA`, `JPG`). Omit to convert every recognized format in the folder |
| `-t`, `--to`          | Target image format (`DDS`, `PNG`, `TGA`, `JPG`). Required unless `--restore`                           |
| `-r`, `--retain`      | Retain original folder and file names                                                                   |
| `-b`, `--bysize`      | Separate converted textures into sub-folders by size                                                    |
| `-c`, `--compression` | Compression effort: `None`, `Fast`, `Balanced`, `Maximum`                                               |
| `-x`, `--restore`     | Restore files to the original format recorded in the source folder's `Result.json`                      |

### Convert Modes

| Mode        | Description                                                               |
| ----------- | ------------------------------------------------------------------------- |
| `Automatic` | Default mode, options are ignored. Writes `Result.json`                   |
| `Manual`    | Honors `--retain` / `--bysize`. With `--retain` also writes `Result.json` |
| `Grouping`  | Groups output by a defined criteria                                       |

### Examples

#### Convert DDS textures to PNG

```pwsh
DDS.Tools convert "D:\DDS-Textures" "D:\PNG-Textures" --to PNG
```

#### Convert PNG images to DDS

```pwsh
DDS.Tools convert "D:\PNG-Textures" "D:\DDS-Textures" --from PNG --to DDS
```

#### Convert DDS to PNG retaining folder structure, separated by size

```pwsh
DDS.Tools convert "D:\DDS-Textures" "D:\PNG-Textures" Manual --to PNG --retain --bysize
```

#### Convert PNG to DDS with maximum compression

```pwsh
DDS.Tools convert "D:\PNG-Textures" "D:\DDS-Textures" --from PNG --to DDS --compression Maximum
```

#### Convert TGA source textures to DDS

```pwsh
DDS.Tools convert "D:\TGA-Textures" "D:\DDS-Textures" --to DDS
```

#### Round-trip a folder that mixes TGA and JPG

Convert every file to PNG for editing, keeping names and folders, then restore each
file to its original format:

```pwsh
# forward: mixed formats -> PNG, Result.json records each file's original format
DDS.Tools convert "D:\Game-Textures" "D:\Edit" Manual --to PNG --retain

# ...edit the PNG files in place...

# restore: PNG -> the original TGA / JPG formats and names
DDS.Tools convert "D:\Edit" "D:\Game-Textures-New" Manual --retain --restore
```

## Contributing

Contributions are welcome! If you have an idea for a new feature, improvement, or bug fix, please follow these steps:

1. Have a look at the [Issues](https://github.com/BoBoBaSs84/DDS.Tools/issues) to see if your idea has already been discussed.
2. If you want to work on an existing issue, please comment on the issue to let others know you're working on it.
3. Fork the repository and create a new branch for your contribution.
4. Make your changes and commit them with clear and descriptive messages.
5. Push your changes to your forked repository and submit a pull request to the main repository.

## Code of Conduct

We expect all contributors to adhere to the [Code of Conduct](CODE_OF_CONDUCT.md).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**Robert Peter Meyer (BoBoBaSs84)**

- GitHub: [@BoBoBaSs84](https://github.com/BoBoBaSs84)
- Repository: [DDS.Tools](https://github.com/BoBoBaSs84/DDS.Tools)
