using BB84.Extensions;

using BCnEncoder.ImageSharp;

using DDS.Tools.Extensions;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models.Base;

using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

using Spectre.Console;

namespace DDS.Tools.Models;

/// <summary>
/// The dds image class.
/// </summary>
/// <param name="ddsDecoder"></param>
/// <param name="logger"></param>
internal sealed class DdsImageModel(DdsDecoder ddsDecoder, ILoggerService<DdsImageModel> logger) : ImageModel, IImageModel
{
	private readonly DdsDecoder _ddsDecoder = ddsDecoder;
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
			AnsiConsole.Markup($"[maroon]{ex.Message}[/]");
		}
	}

	/// <inheritdoc/>
	public override void Save(string filePath)
	{
		try
		{
			FileInfo fileInfo = new(filePath);

			if (fileInfo.Exists)
				throw new ArgumentException($"Already exists: '{filePath}'");

			using FileStream fileStream = File.OpenWrite(filePath);
			PngEncoder pngEncoder = new() { CompressionLevel = PngCompressionLevel.NoCompression };
			_image.SaveAsPng(fileStream, pngEncoder);
		}
		catch (Exception ex)
		{
			_logger.Log(LogExceptionWithParams, filePath, ex);
			AnsiConsole.Markup($"[maroon]{ex.Message}[/]");
		}
	}
}
