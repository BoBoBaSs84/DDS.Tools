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

- **DDS → PNG** conversion of DDS texture files on a large scale
- **PNG → DDS** conversion of PNG image files on a large scale
- Duplicate detection via a persisted result JSON file
- Optional **retain structure** mode to preserve original folder and file names
- Optional **separate by size** mode to sort textures into sub-folders by resolution
- Configurable compression level for both output formats
- Three convert modes: `Automatic`, `Manual`, and `Grouping`
- Interactive CLI with progress spinner powered by [Spectre.Console](https://spectreconsole.net/)

## Usage

DDS.Tools <command> [arguments] [options]

### Commands

| Command | Description                       |
| ------- | --------------------------------- |
| `dds`   | Converts DDS files into PNG files |
| `png`   | Converts PNG files into DDS files |

### Arguments

| Argument         | Description                                               |
| ---------------- | --------------------------------------------------------- |
| `<SourceFolder>` | Path to the folder containing the source images           |
| `<TargetFolder>` | Path to the folder where converted images will be written |
| `[ConvertMode]`  | Optional. Convert mode to use. Default is `Automatic`     |

### Options

| Option                | Description                                          |
| --------------------- | ---------------------------------------------------- |
| `-r`, `--retain`      | Retain original folder and file names                |
| `-b`, `--bysize`      | Separate converted textures into sub-folders by size |
| `-c`, `--compression` | Compression level for the output images              |

### Convert Modes

| Mode        | Description                         |
| ----------- | ----------------------------------- |
| `Automatic` | Default mode, options are ignored   |
| `Manual`    | Manual mode                         |
| `Grouping`  | Groups output by a defined criteria |

### Examples

#### Convert DDS textures to PNG

```pwsh
DDS.Tools dds "D:\DDS-Textures" "D:\PNG-Textures"
```

#### Convert PNG images to DDS

```pwsh
DDS.Tools png "D:\PNG-Textures" "D:\DDS-Textures"
```

#### Convert DDS to PNG retaining folder structure, separated by size

```pwsh
DDS.Tools dds "D:\DDS-Textures" "D:\PNG-Textures" --retain --bysize
```

#### Convert PNG to DDS with a specific compression quality

```pwsh
DDS.Tools png "D:\PNG-Textures" "D:\DDS-Textures" --compression BestQuality
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
