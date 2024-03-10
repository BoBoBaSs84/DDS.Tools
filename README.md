[![CI](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/ci.yml)
[![CodeQL](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/codeql.yml)
[![.NET](https://img.shields.io/badge/net8.0-5C2D91?logo=.NET&labelColor=gray)](https://github.com/BoBoBaSs84/DDS.Tools)
[![C#](https://img.shields.io/badge/12.0-239120?logo=csharp&logoColor=white&labelColor=gray)](https://github.com/BoBoBaSs84/DDS.Tools)
[![Issues](https://img.shields.io/github/issues/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/issues)
[![Commit](https://img.shields.io/github/last-commit/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/commit/main)
[![Size](https://img.shields.io/github/repo-size/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools)
[![License](https://img.shields.io/github/license/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/blob/main/LICENSE)
[![Release](https://img.shields.io/github/v/release/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/releases/latest)

# DDS.Tools

I needed some tools that were able to do some slight lifting so I can do some modding.
This is a simple DDS and PNG tool set that converts DDS images to PNG images and vice versa on a large scale.
Has options for duplicate detection and sorting.

## DDS2PNG

The usage is pretty simple:

`DDS2PNG.exe "D:\Test\textures"`

This will create png files from the dds files searches all sub directories.

A folder named `Result` will be created and catalog file named `Result.json` within the folder.
This is needed for the next tool.

## PNG2DDS

The usage is pretty simple:

`PNG2DDS.exe "D:\Test\Result"`

This will create dds files from the png files, needs the `Result.json` within the folder.
