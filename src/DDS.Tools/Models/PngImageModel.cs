// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BB84.Extensions;

using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;

using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models.Base;
using DDS.Tools.Settings;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Spectre.Console;

namespace DDS.Tools.Models;

/// <summary>
/// Represents a model for PNG image files.
/// </summary>
/// <param name="encoder">The encoder used to encode images into DDS format.</param>
/// <param name="logger">The logger service for logging operations and exceptions.</param>
internal sealed class PngImageModel(DdsEncoder encoder, ILoggerService<PngImageModel> logger) : ImageModel, IImageModel
{
	private readonly DdsEncoder _ddsEncoder = encoder;
	private readonly ILoggerService<PngImageModel> _logger = logger;
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
			_image = Image.Load<Rgba32>(fileStream);
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
			if (settings is not PngConvertSettings pngSettings)
				throw new ArgumentException($"Invalid settings type. Expected {nameof(PngConvertSettings)}.");

			FileInfo fileInfo = new(filePath);

			if (fileInfo.Exists)
				throw new ArgumentException($"Already exists: '{filePath}'");

			if (_image is not null && HasTransparency(_image))
				_ddsEncoder.OutputOptions.Format = CompressionFormat.Bc3;

			_ddsEncoder.OutputOptions.Quality = pngSettings.Compression;

			DdsFile ddsFile = _ddsEncoder.EncodeToDds(_image);
			using FileStream fileStream = File.OpenWrite(filePath);
			ddsFile.Write(fileStream);
		}
		catch (Exception ex)
		{
			_logger.Log(LogExceptionWithParams, filePath, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
		}
	}

	private static bool HasTransparency(Image<Rgba32> image)
	{
		bool hasTransparency = false;
		image.ProcessPixelRows(pixelAccessor =>
		{
			for (int y = 0; y < pixelAccessor.Height; y++)
			{
				Span<Rgba32> row = pixelAccessor.GetRowSpan(y);
				for (int x = 0; x < row.Length; x++)
				{
					Rgba32 pixel = row[x];

					if (pixel.A < byte.MaxValue)
						hasTransparency = true;
				}
			}
		});
		return hasTransparency;
	}
}
