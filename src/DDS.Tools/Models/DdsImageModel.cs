// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BB84.Extensions;

using BCnEncoder.ImageSharp;

using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models.Base;
using DDS.Tools.Settings;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

using Spectre.Console;

namespace DDS.Tools.Models;

/// <summary>
/// Represents a model for DDS image files.
/// </summary>
/// <param name="decoder">The decoder used to decode DDS files into images.</param>
/// <param name="logger">The logger service for logging operations and exceptions.</param>
internal sealed class DdsImageModel(DdsDecoder decoder, ILoggerService<DdsImageModel> logger) : ImageModel, IImageModel
{
	private readonly DdsDecoder _ddsDecoder = decoder;
	private readonly ILoggerService<DdsImageModel> _logger = logger;
	private Image<Rgba32>? _image;

	private static readonly Action<ILogger, string, Exception?> LogExceptionWithParams =
		LoggerMessage.Define<string>(LogLevel.Error, 0, "Exception occured. Params = {Parameters}");

	/// <inheritdoc/>
	public override void Load(string filePath)
	{
		try
		{
			FileInfo fileInfo = new(filePath);

			if (!fileInfo.Exists)
				throw new ArgumentException($"Can't find: '{filePath}'");

			Name = fileInfo.Name;
			Path = fileInfo.FullName;

			using FileStream fileStream = File.OpenRead(filePath);
			_image = _ddsDecoder.DecodeToImageRgba32(fileStream);
			Width = _image.Width;
			Heigth = _image.Height;

			fileStream.Position = 0;
			Data = fileStream.ToByteArray();
			Hash = Data.GetMD5String();
		}
		catch (Exception ex)
		{
			_logger.Log(LogExceptionWithParams, filePath, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
		}
	}

	/// <inheritdoc/>
	public override void Save(string filePath, ConvertSettingsBase settings)
	{
		try
		{
			if (settings is not DdsConvertSettings ddsSettings)
				throw new ArgumentException($"Invalid settings type. Expected {nameof(DdsConvertSettings)}.");

			FileInfo fileInfo = new(filePath);

			if (fileInfo.Exists)
				throw new ArgumentException($"Already exists: '{filePath}'");

			using FileStream fileStream = File.OpenWrite(filePath);
			PngEncoder pngEncoder = new() { CompressionLevel = ddsSettings.Compression };
			_image.SaveAsPng(fileStream, pngEncoder);
		}
		catch (Exception ex)
		{
			_logger.Log(LogExceptionWithParams, filePath, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
		}
	}
}
