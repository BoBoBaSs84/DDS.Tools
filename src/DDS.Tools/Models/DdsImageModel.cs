// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Runtime.InteropServices;

using BB84.Extensions;

using BCnEncoder.Shared.ImageFiles;

using DDS.Tools.Enumerators;
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
/// Represents a model for DDS image files.
/// </summary>
/// <param name="decoder">The decoder used to decode DDS files into images.</param>
/// <param name="logger">The logger service for logging operations and exceptions.</param>
internal sealed class DdsImageModel(DdsDecoder decoder, ILoggerService<DdsImageModel> logger) : ImageModel, IImageModel
{
	private readonly DdsDecoder _ddsDecoder = decoder;
	private readonly ILoggerService<DdsImageModel> _logger = logger;
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
			DdsFile ddsFile = DdsFile.Load(fileStream);

			int width = (int)ddsFile.Faces[0].Width;
			int height = (int)ddsFile.Faces[0].Height;

			BCnEncoder.Shared.ColorRgba32[] pixels = _ddsDecoder.Decode(ddsFile);

			// Convert ColorRgba32[] (RGBA byte layout) into an SKBitmap backed by Rgba8888 memory.
			byte[] rgbaBytes = new byte[width * height * 4];
			int count = Math.Min(pixels.Length, width * height);

			for (int i = 0; i < count; i++)
			{
				rgbaBytes[i * 4] = pixels[i].r;
				rgbaBytes[i * 4 + 1] = pixels[i].g;
				rgbaBytes[i * 4 + 2] = pixels[i].b;
				rgbaBytes[i * 4 + 3] = pixels[i].a;
			}

			var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
			_bitmap = new SKBitmap(info);
			Marshal.Copy(rgbaBytes, 0, _bitmap.GetPixels(), rgbaBytes.Length);

			Width = width;
			Heigth = height;

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

			// Map PngCompressionLevel (0–9) to SkiaSharp PNG quality (100 = no compression, 0 = max compression).
			int quality = MapCompressionLevelToQuality(ddsSettings.Compression);

			using SKImage skImage = SKImage.FromBitmap(_bitmap);
			using SKData encoded = skImage.Encode(SKEncodedImageFormat.Png, quality);
			encoded.SaveTo(fileStream);
		}
		catch (Exception ex)
		{
			_logger.Log(LogExceptionWithParams, filePath, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
		}
	}

	/// <summary>
	/// Maps a <see cref="PngCompressionLevel"/> value (0–9) to a SkiaSharp PNG quality value (0–100).
	/// SkiaSharp quality 100 means least compression; 0 means most compression.
	/// </summary>
	private static int MapCompressionLevelToQuality(PngCompressionLevel level) =>
		(9 - (int)level) * 100 / 9;
}
