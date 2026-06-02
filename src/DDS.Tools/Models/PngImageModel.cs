// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Runtime.InteropServices;

using BB84.Extensions;

using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;

using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models.Base;
using DDS.Tools.Settings;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.Logging;

using SkiaSharp;

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
	private SKBitmap? _bitmap;

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

			// Read raw bytes first, before SkiaSharp consumes the stream.
			Data = fileStream.ToByteArray();
			Hash = Data.GetMD5String();

			fileStream.Position = 0;
			SKBitmap decoded = SKBitmap.Decode(fileStream);

			// Ensure the bitmap is in Rgba8888 format so BCnEncoder receives the expected byte layout.
			if (decoded.ColorType != SKColorType.Rgba8888)
			{
				_bitmap = decoded.Copy(SKColorType.Rgba8888);
				decoded.Dispose();
			}
			else
			{
				_bitmap = decoded;
			}

			Width = _bitmap.Width;
			Heigth = _bitmap.Height;
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

			if (_bitmap is not null && HasTransparency(_bitmap))
				_ddsEncoder.OutputOptions.Format = CompressionFormat.Bc3;

			_ddsEncoder.OutputOptions.Quality = pngSettings.Compression;

			// Extract raw RGBA bytes from the bitmap and encode to DDS using BCnEncoder's raw API.
			byte[] rgbaBytes = new byte[_bitmap!.ByteCount];
			Marshal.Copy(_bitmap.GetPixels(), rgbaBytes, 0, rgbaBytes.Length);

			DdsFile ddsFile = _ddsEncoder.EncodeToDds(rgbaBytes, _bitmap.Width, _bitmap.Height, PixelFormat.Rgba32);
			using FileStream fileStream = File.OpenWrite(filePath);
			ddsFile.Write(fileStream);
		}
		catch (Exception ex)
		{
			_logger.Log(LogExceptionWithParams, filePath, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
		}
	}

	private static bool HasTransparency(SKBitmap bitmap)
	{
		for (int y = 0; y < bitmap.Height; y++)
		{
			for (int x = 0; x < bitmap.Width; x++)
			{
				if (bitmap.GetPixel(x, y).Alpha < byte.MaxValue)
					return true;
			}
		}

		return false;
	}
}
