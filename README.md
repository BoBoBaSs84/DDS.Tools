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
