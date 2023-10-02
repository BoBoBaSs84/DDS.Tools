[![.NET](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/BoBoBaSs84/DDS.Tools/actions/workflows/codeql.yml)
[![Issues](https://img.shields.io/github/issues/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/issues)
[![Commit](https://img.shields.io/github/last-commit/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/commit/main)
[![Forks](https://img.shields.io/github/forks/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/network)
[![Size](https://img.shields.io/github/repo-size/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools)
[![stars](https://img.shields.io/github/stars/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/stargazers)
[![License](https://img.shields.io/github/license/BoBoBaSs84/DDS.Tools)](https://github.com/BoBoBaSs84/DDS.Tools/blob/main/LICENSE)

# DDS.Tools

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
